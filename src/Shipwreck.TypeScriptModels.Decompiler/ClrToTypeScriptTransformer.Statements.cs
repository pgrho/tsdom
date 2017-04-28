using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Ast;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.PatternMatching;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using D = Shipwreck.TypeScriptModels.Declarations;
using E = Shipwreck.TypeScriptModels.Expressions;
using S = Shipwreck.TypeScriptModels.Statements;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    partial class ClrToTypeScriptTransformer
    {
        #region ブロック

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitBlockStatement(BlockStatement blockStatement, string data)
        {
            var bs = new S.BlockStatement();
            bs.Statements = GetStatements(blockStatement, data);

            yield return bs;
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitIfElseStatement(IfElseStatement ifElseStatement, string data)
        {
            var ib = new S.IfStatement();
            ib.Condition = GetExpression(ifElseStatement.Condition, data);
            ib.TruePart = GetStatements(ifElseStatement.TrueStatement, data);
            ib.FalsePart = GetStatements(ifElseStatement.FalseStatement, data);

            yield return ib;
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitForStatement(ForStatement forStatement, string data)
        {
            var fs = new S.ForStatement();

            foreach (var s in forStatement.Initializers)
            {
                var vds = s as VariableDeclarationStatement;
                if (vds != null)
                {
                    foreach (var v in vds.Variables)
                    {
                        var b = new E.ForBindingExpression()
                        {
                            Variable = new E.IdentifierExpression() { Name = v.Name },
                            Initializer = GetExpression(v.Initializer, data)
                        };

                        fs.Initializer = Concat(fs.Initializer, b);
                    }
                }
                else
                {
                    var es = (ExpressionStatement)s;

                    fs.Initializer = Concat(fs.Initializer, GetExpression(es.Expression, data));
                }
            }

            fs.Condition = GetExpression(forStatement.Condition, data);

            foreach (var iter in forStatement.Iterators)
            {
                var es = (ExpressionStatement)iter;

                fs.Iterator = Concat(fs.Iterator, GetExpression(es.Expression, data));
            }

            fs.Statements = GetStatements(forStatement.EmbeddedStatement, data);

            yield return fs;
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitForeachStatement(ForeachStatement foreachStatement, string data)
        {
            var fs = new S.ForOfStatement();
            fs.Variable = new E.IdentifierExpression() { Name = foreachStatement.VariableName };
            fs.Value = GetExpression(foreachStatement.InExpression, data);

            fs.Statements = GetStatements(foreachStatement.EmbeddedStatement, data);

            yield return fs;
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitDoWhileStatement(DoWhileStatement doWhileStatement, string data)
        {
            var bs = new S.DoStatement();
            bs.Condition = GetExpression(doWhileStatement.Condition, data);
            bs.Statements = GetStatements(doWhileStatement.EmbeddedStatement, data);

            yield return bs;
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitWhileStatement(WhileStatement whileStatement, string data)
        {
            var bs = new S.WhileStatement();
            bs.Condition = GetExpression(whileStatement.Condition, data);
            bs.Statements = GetStatements(whileStatement.EmbeddedStatement, data);

            yield return bs;
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitTryCatchStatement(TryCatchStatement tryCatchStatement, string data)
        {
            if (tryCatchStatement.CatchClauses.Count > 1)
            {
                throw new ArgumentException("Too many Catch clauses");
            }

            var s = new S.TryStatement();

            s.TryBlock = GetStatements(tryCatchStatement.TryBlock, data);
            if (tryCatchStatement.CatchClauses.Count == 1)
            {
                var cc = tryCatchStatement.CatchClauses.First();

                s.CatchParameter = new E.IdentifierExpression { Name = string.IsNullOrEmpty(cc.VariableName) ? "__ex" : cc.VariableName };
                s.CatchBlock = GetStatements(cc.Body, data);
            }
            s.FinallyBlock = GetStatements(tryCatchStatement.FinallyBlock, data);

            yield return s;
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitUsingStatement(UsingStatement usingStatement, string data)
        {
            var b = new S.BlockStatement();

            var rvd = usingStatement.ResourceAcquisition as VariableDeclarationStatement;
            var vds = new S.VariableDeclaration();
            vds.Type = S.VariableDeclarationType.Let;
            if (rvd != null)
            {
                foreach (var v in rvd.Variables)
                {
                    vds.Bindings.Add(new S.VariableBinding()
                    {
                        Variable = new E.IdentifierExpression() { Name = v.Name },
                        Type = GetTypeReference(rvd.Type),
                        Initializer = GetExpression(v.Initializer, data)
                    });
                }
            }
            else
            {
                var ex = (ICSharpCode.NRefactory.CSharp.Expression)usingStatement.ResourceAcquisition;
                vds.Bindings.Add(new S.VariableBinding()
                {
                    Variable = new E.IdentifierExpression() { Name = "__usingResource" },
                    Initializer = GetExpression(ex, data)
                });
            }

            b.Statements.Add(vds);
            var tf = new S.TryStatement();
            tf.TryBlock = GetStatements(usingStatement.EmbeddedStatement, data);

            foreach (var bd in vds.Bindings)
            {
                var ib = new S.IfStatement();
                ib.Condition = bd.Variable;

                ib.TruePart.Add(new S.ExpressionStatement()
                {
                    Expression = new E.CallExpression()
                    {
                        Target = new E.PropertyExpression()
                        {
                            Object = bd.Variable,
                            Property = "Dispose"
                        }
                    }
                });

                tf.FinallyBlock.Add(ib);
            }

            b.Statements.Add(tf);

            yield return b;
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitSwitchStatement(SwitchStatement switchStatement, string data)
        {
            var s = new S.SwitchStatement();
            s.Condition = GetExpression(switchStatement.Expression, data);

            foreach (var c in switchStatement.SwitchSections)
            {
                if (c.CaseLabels.Count == 0)
                {
                    continue;
                }
                S.SwitchCase sc = null;
                foreach (var l in c.CaseLabels)
                {
                    sc = new S.SwitchCase();
                    sc.Label = GetExpression(l.Expression, data);
                    s.Cases.Add(sc);
                }
                sc.Statements = GetStatements(c.Statements, data);
            }

            yield return s;
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitLockStatement(LockStatement lockStatement, string data)
        {
            var bs = new S.BlockStatement();
            bs.Statements = GetStatements(lockStatement.EmbeddedStatement, data);
            bs.Statements.Insert(0, new S.ExpressionStatement()
            {
                Expression = GetExpression(lockStatement.Expression, data)
            });

            yield return bs;
        }

        #endregion ブロック

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitEmptyStatement(EmptyStatement emptyStatement, string data)
            => Enumerable.Empty<Syntax>();

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitExpressionStatement(ExpressionStatement expressionStatement, string data)
        {
            yield return new S.ExpressionStatement()
            {
                Expression = GetExpression(expressionStatement.Expression, data)
            };
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitReturnStatement(ReturnStatement returnStatement, string data)
        {
            if (returnStatement.Expression?.IsNull == false)
            {
                yield return new S.ReturnStatement()
                {
                    Value = GetExpression(returnStatement.Expression, data)
                };
            }
            else
            {
                yield return new S.ReturnStatement();
            }
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitBreakStatement(BreakStatement breakStatement, string data)
        {
            yield return new S.BreakStatement();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitContinueStatement(ContinueStatement continueStatement, string data)
        {
            yield return new S.ContinueStatement();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitVariableDeclarationStatement(VariableDeclarationStatement variableDeclarationStatement, string data)
        {
            var vd = new S.VariableDeclaration();
            vd.Type = (variableDeclarationStatement.Modifiers & Modifiers.Const) == Modifiers.Const ? S.VariableDeclarationType.Const : S.VariableDeclarationType.Let;

            foreach (var v in variableDeclarationStatement.Variables)
            {
                vd.Bindings.Add(new S.VariableBinding()
                {
                    Variable = new E.IdentifierExpression() { Name = v.Name },
                    Type = GetTypeReference(variableDeclarationStatement.Type),
                    Initializer = GetExpression(v.Initializer, data)
                });
            }
            if (vd.HasBinding)
            {
                yield return vd;
            }
        }
    }
}
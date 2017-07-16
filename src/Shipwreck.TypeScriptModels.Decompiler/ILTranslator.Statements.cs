using ICSharpCode.NRefactory.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using E = Shipwreck.TypeScriptModels.Expressions;
using S = Shipwreck.TypeScriptModels.Statements;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    partial class ILTranslator
    {
        #region ブロック

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitBlockStatement(BlockStatement blockStatement, ILTranslationContext data)
            => OnVisiting(data, blockStatement, VisitingBlockStatement)
            ?? OnVisited(data, blockStatement, VisitedBlockStatement, TranslateBlockStatement(blockStatement, data));

        protected virtual IEnumerable<Syntax> TranslateBlockStatement(BlockStatement blockStatement, ILTranslationContext data)
        {
            var bs = new S.BlockStatement();
            bs.Statements = GetStatements(blockStatement, data);

            yield return bs;
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitIfElseStatement(IfElseStatement ifElseStatement, ILTranslationContext data)
            => OnVisiting(data, ifElseStatement, VisitingIfElseStatement)
            ?? OnVisited(data, ifElseStatement, VisitedIfElseStatement, TranslateIfElseStatement(ifElseStatement, data));

        protected virtual IEnumerable<Syntax> TranslateIfElseStatement(IfElseStatement ifElseStatement, ILTranslationContext data)
        {
            var ib = new S.IfStatement();
            ib.Condition = GetExpression(ifElseStatement.Condition, data);
            ib.TruePart = GetStatements(ifElseStatement.TrueStatement, data);
            ib.FalsePart = GetStatements(ifElseStatement.FalseStatement, data);

            yield return ib;
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitForStatement(ForStatement forStatement, ILTranslationContext data)
            => OnVisiting(data, forStatement, VisitingForStatement)
            ?? OnVisited(data, forStatement, VisitedForStatement, TranslateForStatement(forStatement, data));

        protected virtual IEnumerable<Syntax> TranslateForStatement(ForStatement forStatement, ILTranslationContext data)
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

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitForeachStatement(ForeachStatement foreachStatement, ILTranslationContext data)
            => OnVisiting(data, foreachStatement, VisitingForeachStatement)
            ?? OnVisited(data, foreachStatement, VisitedForeachStatement, TranslateForeachStatement(foreachStatement, data));

        protected virtual IEnumerable<Syntax> TranslateForeachStatement(ForeachStatement foreachStatement, ILTranslationContext data)
        {
            var fs = new S.ForOfStatement();
            fs.Variable = new E.IdentifierExpression() { Name = foreachStatement.VariableName };
            fs.Value = GetExpression(foreachStatement.InExpression, data);

            fs.Statements = GetStatements(foreachStatement.EmbeddedStatement, data);

            yield return fs;
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitDoWhileStatement(DoWhileStatement doWhileStatement, ILTranslationContext data)
            => OnVisiting(data, doWhileStatement, VisitingDoWhileStatement)
            ?? OnVisited(data, doWhileStatement, VisitedDoWhileStatement, TranslateDoWhileStatement(doWhileStatement, data));

        protected virtual IEnumerable<Syntax> TranslateDoWhileStatement(DoWhileStatement doWhileStatement, ILTranslationContext data)
        {
            var bs = new S.DoStatement();
            bs.Condition = GetExpression(doWhileStatement.Condition, data);
            bs.Statements = GetStatements(doWhileStatement.EmbeddedStatement, data);

            yield return bs;
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitWhileStatement(WhileStatement whileStatement, ILTranslationContext data)
            => OnVisiting(data, whileStatement, VisitingWhileStatement)
            ?? OnVisited(data, whileStatement, VisitedWhileStatement, TranslateWhileStatement(whileStatement, data));

        protected virtual IEnumerable<Syntax> TranslateWhileStatement(WhileStatement whileStatement, ILTranslationContext data)
        {
            var bs = new S.WhileStatement();
            bs.Condition = GetExpression(whileStatement.Condition, data);
            bs.Statements = GetStatements(whileStatement.EmbeddedStatement, data);

            yield return bs;
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitTryCatchStatement(TryCatchStatement tryCatchStatement, ILTranslationContext data)
            => OnVisiting(data, tryCatchStatement, VisitingTryCatchStatement)
            ?? OnVisited(data, tryCatchStatement, VisitedTryCatchStatement, TranslateTryCatchStatement(tryCatchStatement, data));

        protected virtual IEnumerable<Syntax> TranslateTryCatchStatement(TryCatchStatement tryCatchStatement, ILTranslationContext data)
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

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitUsingStatement(UsingStatement usingStatement, ILTranslationContext data)
            => OnVisiting(data, usingStatement, VisitingUsingStatement)
            ?? OnVisited(data, usingStatement, VisitedUsingStatement, TranslateUsingStatement(usingStatement, data));

        protected virtual IEnumerable<Syntax> TranslateUsingStatement(UsingStatement usingStatement, ILTranslationContext data)
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
                        Type = ResolveType(v, rvd.Type),
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

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitSwitchStatement(SwitchStatement switchStatement, ILTranslationContext data)
            => OnVisiting(data, switchStatement, VisitingSwitchStatement)
            ?? OnVisited(data, switchStatement, VisitedSwitchStatement, TranslateSwitchStatement(switchStatement, data));

        protected virtual IEnumerable<Syntax> TranslateSwitchStatement(SwitchStatement switchStatement, ILTranslationContext data)
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

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitLockStatement(LockStatement lockStatement, ILTranslationContext data)
            => OnVisiting(data, lockStatement, VisitingLockStatement)
            ?? OnVisited(data, lockStatement, VisitedLockStatement, TranslateLockStatement(lockStatement, data));

        protected virtual IEnumerable<Syntax> TranslateLockStatement(LockStatement lockStatement, ILTranslationContext data)
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

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitEmptyStatement(EmptyStatement emptyStatement, ILTranslationContext data)
            => Enumerable.Empty<Syntax>();

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitExpressionStatement(ExpressionStatement expressionStatement, ILTranslationContext data)
            => OnVisiting(data, expressionStatement, VisitingExpressionStatement)
            ?? OnVisited(data, expressionStatement, VisitedExpressionStatement, TranslateExpressionStatement(expressionStatement, data));

        protected virtual IEnumerable<Syntax> TranslateExpressionStatement(ExpressionStatement expressionStatement, ILTranslationContext data)
        {
            yield return new S.ExpressionStatement()
            {
                Expression = GetExpression(expressionStatement.Expression, data)
            };
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitReturnStatement(ReturnStatement returnStatement, ILTranslationContext data)
            => OnVisiting(data, returnStatement, VisitingReturnStatement)
            ?? OnVisited(data, returnStatement, VisitedReturnStatement, TranslateReturnStatement(returnStatement, data));

        protected virtual IEnumerable<Syntax> TranslateReturnStatement(ReturnStatement returnStatement, ILTranslationContext data)
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

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitBreakStatement(BreakStatement breakStatement, ILTranslationContext data)
            => OnVisiting(data, breakStatement, VisitingBreakStatement)
            ?? OnVisited(data, breakStatement, VisitedBreakStatement, TranslateBreakStatement(breakStatement, data));

        protected virtual IEnumerable<Syntax> TranslateBreakStatement(BreakStatement breakStatement, ILTranslationContext data)
        {
            yield return new S.BreakStatement();
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitContinueStatement(ContinueStatement continueStatement, ILTranslationContext data)
            => OnVisiting(data, continueStatement, VisitingContinueStatement)
            ?? OnVisited(data, continueStatement, VisitedContinueStatement, TranslateContinueStatement(continueStatement, data));

        protected virtual IEnumerable<Syntax> TranslateContinueStatement(ContinueStatement continueStatement, ILTranslationContext data)
        {
            yield return new S.ContinueStatement();
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitVariableDeclarationStatement(VariableDeclarationStatement variableDeclarationStatement, ILTranslationContext data)
            => OnVisiting(data, variableDeclarationStatement, VisitingVariableDeclarationStatement)
            ?? OnVisited(data, variableDeclarationStatement, VisitedVariableDeclarationStatement, TranslateVariableDeclarationStatement(variableDeclarationStatement, data));

        protected virtual IEnumerable<Syntax> TranslateVariableDeclarationStatement(VariableDeclarationStatement variableDeclarationStatement, ILTranslationContext data)
        {
            var vd = new S.VariableDeclaration();
            vd.Type = (variableDeclarationStatement.Modifiers & Modifiers.Const) == Modifiers.Const ? S.VariableDeclarationType.Const : S.VariableDeclarationType.Let;

            foreach (var v in variableDeclarationStatement.Variables)
            {
                vd.Bindings.Add(new S.VariableBinding()
                {
                    Variable = new E.IdentifierExpression() { Name = v.Name },
                    Type = ResolveType(v, variableDeclarationStatement.Type),
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
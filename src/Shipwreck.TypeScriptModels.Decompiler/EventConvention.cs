using ICSharpCode.NRefactory.CSharp;
using System.Collections.ObjectModel;
using System.Linq;
using M = Shipwreck.TypeScriptModels;
using D = Shipwreck.TypeScriptModels.Declarations;
using S = Shipwreck.TypeScriptModels.Statements;
using E = Shipwreck.TypeScriptModels.Expressions;
using Mono.Cecil;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    public sealed class EventConvention : ILTranslationConvention
    {
        public override void ApplyTo(ILTranslator translator)
        {
            translator.VisitingEventDeclaration -= Translator_VisitingEventDeclaration;
            translator.VisitingEventDeclaration += Translator_VisitingEventDeclaration;

            translator.VisitedAssignmentExpression -= Translator_VisitedAssignmentExpression;
            translator.VisitedAssignmentExpression += Translator_VisitedAssignmentExpression;
        }

        private void Translator_VisitingEventDeclaration(object sender, VisitingEventArgs<EventDeclaration> e)
        {
            if (e.Handled)
            {
                return;
            }

            var ilt = (ILTranslator)sender;

            var tr = ilt.ResolveType(e.Node, e.Node.ReturnType);
            var at = new D.ArrayType(tr);

            var n = e.Node.Variables.Single().Name;
            var fn = "__" + n;

            var fd = new D.FieldDeclaration()
            {
                FieldName = fn,
                FieldType = at
            };

            var ad = new D.MethodDeclaration()
            {
                MethodName = "add_" + n,
                Parameters = new Collection<D.Parameter>()
                {
                    new D.Parameter()
                    {
                        ParameterName = "value",
                        ParameterType = tr
                    }
                }
            };

            var fr = new E.ThisExpression().Property(fn);
            ad.Statements.Add(
                    fr.LogicalOrWith(fr.AssignedBy(new E.ArrayExpression()))
                        .Property("push")
                        .Call(new E.IdentifierExpression("value"))
                        .ToStatement());

            var rd = new D.MethodDeclaration()
            {
                MethodName = "remove_" + n,
                Parameters = new Collection<D.Parameter>()
                {
                    new D.Parameter()
                    {
                        ParameterName = "value",
                        ParameterType = tr
                    }
                }
            };

            rd.Statements.Add(new S.IfStatement()
            {
                Condition = fr,
                TruePart = new Collection<Statement>()
                {
                    fr.AssignedBy(fr.Property("map").Call(new E.ArrowFunctionExpression()
                    {
                        Parameters =new Collection<D.Parameter> ()
                        {
                            new D.Parameter()
                            {
                                ParameterName = "e",
                               // ParameterType = tr
                            }
                        },
                        Statements = new Collection<Statement> ()
                        {
                            new E.IdentifierExpression("e").IsStrictNotEqualTo(new  E.IdentifierExpression("value")).ToReturn()
                        }
                    })  ).ToStatement()
                }
            });

            e.Results = new Syntax[] { fd, ad, rd };
        }


        private void Translator_VisitedAssignmentExpression(object sender, VisitedEventArgs<ICSharpCode.NRefactory.CSharp.AssignmentExpression> e)
        {
            if (e.Handled || e.Results.Count != 1)
            {
                return;
            }

            var ae = e.Results[0] as E.AssignmentExpression;

            if (ae == null)
            {
                return;
            }

            if (e.Node.Operator == AssignmentOperatorType.Add
                || e.Node.Operator == AssignmentOperatorType.Subtract)
            {
                var mre = e.Node.Left as MemberReferenceExpression;
                if (mre != null)
                {
                    var ed = mre.Annotation<EventDefinition>();
                    if (ed != null)
                    {
                        var t = mre.Target.AcceptVisitor((ILTranslator)sender, e.Context).ToArray();
                        if (t.Length == 1)
                        {
                            var v = e.Node.Left.AcceptVisitor((ILTranslator)sender, e.Context).ToArray();
                            if (v.Length == 1)
                            {
                                var te = (Expression)t[0];
                                e.Results = new[] { te.Property((e.Node.Operator == AssignmentOperatorType.Add ? "add_" : "remove_") + ed.Name).Call((Expression)v[0]) };
                            }
                        }
                    }
                }
            }
        }

    }
}
using ICSharpCode.NRefactory.CSharp;
using System.Collections.ObjectModel;
using System.Linq;
using M = Shipwreck.TypeScriptModels;
using D = Shipwreck.TypeScriptModels.Declarations;
using S = Shipwreck.TypeScriptModels.Statements;
using E = Shipwreck.TypeScriptModels.Expressions;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    public sealed class EventConvention : ILTranslationConvention
    {
        public override void ApplyTo(ILTranslator translator)
        {
            translator.VisitingEventDeclaration += Translator_VisitingEventDeclaration;
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
    }
}
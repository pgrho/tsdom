using ICSharpCode.Decompiler.Ast;
using ICSharpCode.NRefactory.CSharp;
using Mono.Cecil;
using System.Collections.ObjectModel;
using System.Linq;
using D = Shipwreck.TypeScriptModels.Declarations;
using E = Shipwreck.TypeScriptModels.Expressions;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    public sealed class EventConvention : ILTranslationConvention
    {
        internal const string FIELD_PREFIX = "$ev_";
        internal const string ADD_PREFIX = "$addev_";
        internal const string REMOVE_PREFIX = "$remev_";

        public override void ApplyTo(ILTranslator translator)
        {
            translator.VisitingEventDeclaration -= Translator_VisitingEventDeclaration;
            translator.VisitingEventDeclaration += Translator_VisitingEventDeclaration;

            translator.VisitingCustomEventDeclaration -= Translator_VisitingCustomEventDeclaration;
            translator.VisitingCustomEventDeclaration += Translator_VisitingCustomEventDeclaration;

            translator.VisitingMemberReferenceExpression -= Translator_VisitingMemberReferenceExpression;
            translator.VisitingMemberReferenceExpression += Translator_VisitingMemberReferenceExpression;

            translator.VisitedAssignmentExpression -= Translator_VisitedAssignmentExpression;
            translator.VisitedAssignmentExpression += Translator_VisitedAssignmentExpression;
        }

        private void Translator_VisitingEventDeclaration(object sender, VisitingEventArgs<EventDeclaration> e)
        {
            if (e.Handled)
            {
                return;
            }

            var translator = (ILTranslator)sender;

            var ed = e.Node.Annotation<EventDefinition>();
            if (ed == null)
            {
                return;
            }

            var delType = translator.ResolveType(e.Node, e.Node.ReturnType);
            var arrayType = delType as D.ArrayType;
            if (arrayType == null)
            {
                arrayType = (delType as D.UnionType)?.ElementTypes.OfType<D.ArrayType>().FirstOrDefault();
                if (arrayType == null)
                {
                    arrayType = delType.MakeArrayType();
                }
            }

            var n = e.Node.Variables.Single().Name;
            var fn = FIELD_PREFIX + n;
            var fd = new D.FieldDeclaration()
            {
                FieldName = fn,
                FieldType = arrayType
            };

            var ad = CreateAddAccessor(translator, e, arrayType, ed);
            var rd = CreateRemoveAccessor(translator, e, arrayType, ed);

            e.Results = new Syntax[] { fd, ad, rd };
        }

        private D.MethodDeclaration CreateAddAccessor(ILTranslator translator, VisitingEventArgs<EventDeclaration> e, D.ArrayType arrayType, EventDefinition ed)
        {
            var n = e.Node.Variables.Single().Name;
            var fn = FIELD_PREFIX + n;

            var union = arrayType.UnionWith(arrayType.ElementType);
            var ad = new D.MethodDeclaration()
            {
                MethodName = ADD_PREFIX + n,
                Parameters = new Collection<D.Parameter>()
                {
                    new D.Parameter()
                    {
                        ParameterName = "value",
                        ParameterType = union
                    }
                }
            };

            var mre = new MemberReferenceExpression()
            {
                Target = new ThisReferenceExpression(),
                MemberName = fn
            };
            var ve = new IdentifierExpression("value");
            mre.AddAnnotation(new TypeInformation(ed.EventType, ed.EventType));

            ad.Statements = translator.GetStatements(new ExpressionStatement()
            {
                Expression = new AssignmentExpression()
                {
                    Operator = AssignmentOperatorType.Add,
                    Left = mre,
                    Right = ve
                }
            }, e.Context);

            return ad;
        }

        private D.MethodDeclaration CreateRemoveAccessor(ILTranslator translator, VisitingEventArgs<EventDeclaration> e, D.ArrayType arrayType, EventDefinition ed)
        {
            var n = e.Node.Variables.Single().Name;
            var fn = FIELD_PREFIX + n;

            var union = arrayType.UnionWith(arrayType.ElementType);
            var rd = new D.MethodDeclaration()
            {
                MethodName = REMOVE_PREFIX + n,
                Parameters = new Collection<D.Parameter>()
                {
                    new D.Parameter()
                    {
                        ParameterName = "value",
                        ParameterType = union
                    }
                }
            };
            var mre = new MemberReferenceExpression()
            {
                Target = new ThisReferenceExpression(),
                MemberName = fn
            };
            var ve = new IdentifierExpression("value");
            mre.AddAnnotation(new TypeInformation(ed.EventType, ed.EventType));

            rd.Statements = translator.GetStatements(new ExpressionStatement()
            {
                Expression = new AssignmentExpression()
                {
                    Operator = AssignmentOperatorType.Subtract,
                    Left = mre,
                    Right = ve
                }
            }, e.Context);

            return rd;
        }

        private void Translator_VisitingCustomEventDeclaration(object sender, VisitingEventArgs<CustomEventDeclaration> e)
        {
            if (e.Handled)
            {
                return;
            }

            var ilt = (ILTranslator)sender;

            var tr = ilt.ResolveType(e.Node, e.Node.ReturnType);
            var at = new D.ArrayType(tr);

            var n = e.Node.Name;

            var ad = new D.MethodDeclaration()
            {
                MethodName = ADD_PREFIX + n,
                Parameters = new Collection<D.Parameter>()
                {
                    new D.Parameter()
                    {
                        ParameterName = "value",
                        ParameterType = tr
                    }
                }
            };

            ad.Statements = ilt.GetStatements(e.Node.AddAccessor.Body, e.Context);

            var rd = new D.MethodDeclaration()
            {
                MethodName = REMOVE_PREFIX + n,
                Parameters = new Collection<D.Parameter>()
                {
                    new D.Parameter()
                    {
                        ParameterName = "value",
                        ParameterType = tr
                    }
                }
            };

            rd.Statements = ilt.GetStatements(e.Node.RemoveAccessor.Body, e.Context);

            e.Results = new Syntax[] { ad, rd };
        }

        private void Translator_VisitingMemberReferenceExpression(object sender, VisitingEventArgs<MemberReferenceExpression> e)
        {
            if (e.Handled)
            {
                return;
            }
            var ed = e.Node.Annotation<EventDefinition>();
            if (ed != null)
            {
                var tar = e.Node.Target.AcceptVisitor((ILTranslator)sender, e.Context).ToArray();
                if (tar.Length == 1)
                {
                    e.Results = new[] { ((Expression)tar[0]).Property(FIELD_PREFIX + ed.Name) };
                }
            }
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

            var op = e.Node.Operator;
            var isAdd = op == AssignmentOperatorType.Add;
            if (!isAdd
                && op != AssignmentOperatorType.Subtract)
            {
                return;
            }

            var mre = e.Node.Left as MemberReferenceExpression;
            if (mre == null)
            {
                return;
            }

            var ed = mre.Annotation<EventDefinition>();
            if (ed == null)
            {
                return;
            }
            var pe = ae.Target as E.PropertyExpression;
            Expression tar;
            if (pe != null)
            {
                tar = pe.Object;
            }
            else
            {
                var t = mre.Target.AcceptVisitor((ILTranslator)sender, e.Context).ToArray();
                if (t.Length != 1)
                {
                    return;
                }
                tar = (Expression)t[0];
            }
            e.Results = new[] { tar.Property((isAdd ? ADD_PREFIX : REMOVE_PREFIX) + ed.Name).Call(ae.Value) };
        }
    }
}
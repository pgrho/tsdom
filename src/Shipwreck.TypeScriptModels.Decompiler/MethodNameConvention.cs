using ICSharpCode.NRefactory.CSharp;
using System.Linq;
using System.Reflection;
using D = Shipwreck.TypeScriptModels.Declarations;
using E = Shipwreck.TypeScriptModels.Expressions;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    public sealed class MethodNameConvention : MethodLookupConvention
    {
        private readonly string _MethodName;
        private readonly string _TypeName;

        public MethodNameConvention(MethodInfo targetMethod, string methodName, string typeName = null)
            : base(targetMethod)
        {
            _MethodName = methodName;
            _TypeName = typeName;
        }

        protected override void OnMethodDeclarationMatched(ILTranslator sender, VisitedEventArgs<MethodDeclaration> e)
        {
            if (e.Results.Count() == 1)
            {
                var method = e.Results.Single() as D.MethodDeclaration;
                if (method != null)
                {
                    method.MethodName = _MethodName;
                    e.Handled = true;
                }
            }
        }

        protected override void OnInvocationExpressionMatched(ILTranslator sender, VisitedEventArgs<InvocationExpression> e)
        {
            if (e.Results.Count() == 1)
            {
                var call = e.Results.Single() as E.CallExpression;
                if (call != null)
                {
                    var pe = call.Target as E.PropertyExpression;

                    if (pe != null)
                    {
                        pe.Property = _MethodName;

                        if (_TypeName != null)
                        {
                            pe.Object = new E.IdentifierExpression()
                            {
                                Name = _TypeName
                            };
                        }
                        e.Handled = true;
                    }
                }
            }
        }
    }
}
using ICSharpCode.NRefactory.CSharp;
using System;
using System.Linq;
using System.Reflection;
using D = Shipwreck.TypeScriptModels.Declarations;
using E = Shipwreck.TypeScriptModels.Expressions;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    public sealed class MethodNameConvention : MethodLookupConvention
    {
        private readonly string _ResultMethodName;
        private readonly string _ResultTypeName;

        public MethodNameConvention(MethodInfo targetMethod, string resultMethodName, string resultTypeName = null)
            : base(targetMethod)
        {
            _ResultMethodName = resultMethodName;
            _ResultTypeName = resultTypeName;
        }

        public MethodNameConvention(Type targetType, string targetMethodName, string resultMethodName, string resultTypeName = null)
         : base(targetType, targetMethodName)
        {
            _ResultMethodName = resultMethodName;
            _ResultTypeName = resultTypeName;
        }

        protected override void OnMethodDeclarationMatched(ILTranslator sender, VisitedEventArgs<MethodDeclaration> e)
        {
            if (e.Results.Count() == 1)
            {
                var method = e.Results.Single() as D.MethodDeclaration;
                if (method != null)
                {
                    method.MethodName = _ResultMethodName;
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
                        pe.Property = _ResultMethodName;

                        if (_ResultTypeName != null)
                        {
                            pe.Object = new E.IdentifierExpression()
                            {
                                Name = _ResultTypeName
                            };
                        }
                        e.Handled = true;
                    }
                }
            }
        }
    }
}
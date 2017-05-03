using ICSharpCode.NRefactory.CSharp;
using System.Reflection;

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
            if (e.Handled)
            {
                return;
            }
            // TODO: ŽÀ‘•
        }

        protected override void OnInvocationExpressionMatched(ILTranslator sender, VisitedEventArgs<InvocationExpression> e)
        {
            if (e.Handled)
            {
                return;
            }
            // TODO: ŽÀ‘•
        }
    }
}
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using System;
using System.Linq;
using System.Reflection;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    public abstract class MethodLookupConvention : ILTranslationConvention
    {
        protected MethodLookupConvention(MethodInfo targetMethod)
        {
            TargetMethod = targetMethod;
        }

        protected MethodInfo TargetMethod { get; }

        public override void ApplyTo(ILTranslator translator)
        {
            translator.VisitedMethodDeclaration -= Translator_VisitedMethodDeclaration;
            translator.VisitedMethodDeclaration += Translator_VisitedMethodDeclaration;

            translator.VisitedInvocationExpression -= Translator_VisitedInvocationExpression;
            translator.VisitedInvocationExpression += Translator_VisitedInvocationExpression;
        }

        private void Translator_VisitedMethodDeclaration(object sender, VisitedEventArgs<MethodDeclaration> e)
        {
            var md = e.Node;

            if (TargetMethod.DeclaringType.IsEquivalentTo(md.Parent as TypeDeclaration)
                && md.Name == TargetMethod.Name // TODO: ジェネリックメソッドの検証
                && md.TypeParameters.Select(tp => tp.Name).SequenceEqual(TargetMethod.GetGenericArguments().Select(tp => tp.Name)) // TODO: 型の一致を検証
                && md.Parameters.Select(tp => tp.Name).SequenceEqual(TargetMethod.GetParameters().Select(tp => tp.Name)))
            {
                OnMethodDeclarationMatched((ILTranslator)sender, e);
            }
        }

        private void Translator_VisitedInvocationExpression(object sender, VisitedEventArgs<InvocationExpression> e)
        {
            var mre = e.Node.Target as MemberReferenceExpression;
            if (mre != null)
            {
                var rr = e.Context.AstResolver.Resolve(mre.Target);

                // TODO: 引数の検証
                if (TargetMethod.DeclaringType.IsEquivalentTo(rr.Type)
                    && mre.MemberName == TargetMethod.Name)
                {
                    OnInvocationExpressionMatched((ILTranslator)sender, e);
                }
            }
        }

        protected abstract void OnMethodDeclarationMatched(ILTranslator sender, VisitedEventArgs<MethodDeclaration> e);

        protected abstract void OnInvocationExpressionMatched(ILTranslator sender, VisitedEventArgs<InvocationExpression> e);
    }
}
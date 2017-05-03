using ICSharpCode.NRefactory.CSharp;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    public class MethodLookupConvention : ILTranslationConvention
    {
        public override void ApplyTo(ILTranslator translator)
        {
            translator.VisitedMethodDeclaration -= Translator_VisitedMethodDeclaration;
            translator.VisitedMethodDeclaration += Translator_VisitedMethodDeclaration;

            translator.VisitedInvocationExpression -= Translator_VisitedInvocationExpression;
            translator.VisitedInvocationExpression += Translator_VisitedInvocationExpression;
        }

        private void Translator_VisitedMethodDeclaration(object sender, TranslationEventArgs<MethodDeclaration> e)
        {
            if (e.Results == null)
            {
                // TODO: 実装
            }
        }

        private void Translator_VisitedInvocationExpression(object sender, TranslationEventArgs<InvocationExpression> e)
        {
            if (e.Results == null)
            {
                // TODO: 実装
            }
        }
    }
}
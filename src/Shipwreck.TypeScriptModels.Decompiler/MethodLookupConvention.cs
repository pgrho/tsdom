using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using System;
using System.Linq;
using System.Reflection;
using Mono.Cecil;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    public abstract class MethodLookupConvention : ILTranslationConvention
    {
        protected MethodLookupConvention(Type targetType, string targetMethodName)
        {
            TargetType = targetType;
            TargetMethodName = targetMethodName;
        }
        protected MethodLookupConvention(MethodInfo targetMethod)
        {
            TargetMethod = targetMethod;
        }

        protected Type TargetType { get; }
        protected string TargetMethodName { get; }

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
            if (e.Handled)
            {
                return;
            }

            var mr = e.Node.Annotation<MethodDefinition>();

            if ((TargetMethod != null && TargetMethod.IsEquivalentTo(mr))
                || (TargetType != null && TargetType.IsBaseTypeOf(mr.DeclaringType) && TargetMethodName == mr.Name))
            {
                OnMethodDeclarationMatched((ILTranslator)sender, e);
            }
        }

        private void Translator_VisitedInvocationExpression(object sender, VisitedEventArgs<InvocationExpression> e)
        {
            if (e.Handled)
            {
                return;
            }

            var mr = e.Node.Annotation<MethodReference>();

            if (TargetMethod.IsEquivalentTo(mr)
                || (TargetType != null && TargetType.IsBaseTypeOf(mr.DeclaringType) && TargetMethodName == mr.Name))
            {
                OnInvocationExpressionMatched((ILTranslator)sender, e);
            }
        }

        protected abstract void OnMethodDeclarationMatched(ILTranslator sender, VisitedEventArgs<MethodDeclaration> e);

        protected abstract void OnInvocationExpressionMatched(ILTranslator sender, VisitedEventArgs<InvocationExpression> e);
    }
}
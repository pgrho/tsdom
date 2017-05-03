using ICSharpCode.NRefactory.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    partial class ILTranslator
    {
		public event EventHandler<TranslationEventArgs<MethodDeclaration>> VisitingMethodDeclaration;
		public event EventHandler<TranslationEventArgs<MethodDeclaration>> VisitedMethodDeclaration;
		public event EventHandler<TranslationEventArgs<InvocationExpression>> VisitingInvocationExpression;
		public event EventHandler<TranslationEventArgs<InvocationExpression>> VisitedInvocationExpression;
	}
}
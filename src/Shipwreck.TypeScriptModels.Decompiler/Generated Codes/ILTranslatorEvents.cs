using ICSharpCode.NRefactory.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    partial class ILTranslator
    {
		public event EventHandler<VisitingEventArgs<MethodDeclaration>> VisitingMethodDeclaration;
		public event EventHandler<VisitedEventArgs<MethodDeclaration>> VisitedMethodDeclaration;
		public event EventHandler<VisitingEventArgs<InvocationExpression>> VisitingInvocationExpression;
		public event EventHandler<VisitedEventArgs<InvocationExpression>> VisitedInvocationExpression;
	}
}
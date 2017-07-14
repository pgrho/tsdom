using ICSharpCode.NRefactory.CSharp;
using System.Collections.Generic;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    public class VisitingEventArgs<T> where T : AstNode
    {
        public VisitingEventArgs(ILTransformationContext context, T node)
        {
            Context = context;
            Node = node;
        }

        public ILTransformationContext Context { get; }

        public T Node { get; }

        public IEnumerable<Syntax> Results { get; set; }

        public bool Handled
            => Results != null;
    }

}
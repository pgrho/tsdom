using ICSharpCode.NRefactory.CSharp;
using System.Collections.Generic;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    public class VisitingEventArgs<T> where T : AstNode
    {
        public VisitingEventArgs(ILTranslationContext context, T node)
        {
            Context = context;
            Node = node;
        }

        public ILTranslationContext Context { get; }

        public T Node { get; }

        public IEnumerable<Syntax> Results { get; set; }

        public bool Handled
            => Results != null;
    }

}
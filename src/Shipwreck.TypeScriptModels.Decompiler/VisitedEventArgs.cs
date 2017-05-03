using ICSharpCode.NRefactory.CSharp;
using System.Collections.Generic;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    public class VisitedEventArgs<T> where T : AstNode
    {
        private readonly IEnumerable<Syntax> _OriginalResults;

        public VisitedEventArgs(ILTransformationContext context, T node, IEnumerable<Syntax> results)
        {
            Context = context;
            Node = node;
            Results = results;
            _OriginalResults = results;
        }

        public ILTransformationContext Context { get; }

        public T Node { get; }

        public IEnumerable<Syntax> Results { get; set; }

        public bool Handled
            => Results != _OriginalResults;
    }
}
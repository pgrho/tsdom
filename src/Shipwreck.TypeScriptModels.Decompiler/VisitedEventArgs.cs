using ICSharpCode.NRefactory.CSharp;
using System.Collections.Generic;
using System.Linq;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    public class VisitedEventArgs<T> where T : AstNode
    {
        private readonly IEnumerable<Syntax> _OriginalResults;
        public bool? _Handled;

        public VisitedEventArgs(ILTranslationContext context, T node, IEnumerable<Syntax> results)
        {
            Context = context;
            Node = node;
            var c = (results as ICollection<Syntax>) ?? results.ToArray();
            Results = (c as IList<Syntax>) ?? c.ToList();
            _OriginalResults = Results;
        }

        public ILTranslationContext Context { get; }

        public T Node { get; }

        public IList<Syntax> Results { get; set; }

        public bool Handled
        {
            get
            {
                return _Handled ?? Results != _OriginalResults;
            }
            set
            {
                _Handled = value;
            }
        }
    }
}
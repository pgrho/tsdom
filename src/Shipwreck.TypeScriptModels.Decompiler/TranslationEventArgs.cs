using ICSharpCode.NRefactory.CSharp;
using System.Collections.Generic;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    public class TranslationEventArgs<T> where T : AstNode
    {
        public TranslationEventArgs(ILTransformationContext context, T node)
        {
            Context = context;
            Node = node;
        }

        public ILTransformationContext Context { get; }

        public T Node { get; }

        public IEnumerable<Syntax> Results { get; set; }
    }

    public static class TranslationEventArgs
    {
        public static TranslationEventArgs<T> Create<T>(ILTransformationContext context, T node, IEnumerable<Syntax> results = null)
            where T : AstNode
            => new TranslationEventArgs<T>(context, node)
            {
                Results = results
            };
    }
}
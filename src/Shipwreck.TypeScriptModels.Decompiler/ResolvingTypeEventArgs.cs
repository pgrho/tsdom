using ICSharpCode.NRefactory.CSharp;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    public class ResolvingTypeEventArgs<T>
        where T : class
    {
        public ResolvingTypeEventArgs(AstNode node, T originalType)
        {
            Node = node;
            OriginalType = originalType;
        }

        public AstNode Node { get; }

        public T OriginalType { get; }

        public ITypeReference Result { get; set; }

        public bool? IsOptional { get; set; }
    }
}
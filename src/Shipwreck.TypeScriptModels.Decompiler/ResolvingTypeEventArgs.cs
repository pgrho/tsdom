namespace Shipwreck.TypeScriptModels.Decompiler
{
    public class ResolvingTypeEventArgs<T>
        where T : class
    {
        public ResolvingTypeEventArgs(T originalType)
        {
            OriginalType = originalType;
        }

        public T OriginalType { get; }

        public ITypeReference Result { get; set; }

        public bool? IsOptional { get; set; }
    }
}
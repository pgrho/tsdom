namespace Shipwreck.TypeScriptModels.Decompiler
{
    public sealed class ILTransformationContext
    {
        public bool HasYield { get; set; }
        public bool HasAwait { get; set; }

        public ILTransformationContext GetChildContext()
            => new ILTransformationContext();
    }
}
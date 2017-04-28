namespace Shipwreck.TypeScriptModels.Decompiler
{
    public sealed class ClrToTypeScriptTransformationContext
    {
        public bool HasYield { get; set; }
        public bool HasAwait { get; set; }

        public ClrToTypeScriptTransformationContext GetChildContext()
            => new ClrToTypeScriptTransformationContext();
    }
}
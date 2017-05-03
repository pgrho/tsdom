namespace Shipwreck.TypeScriptModels.Decompiler
{
    public abstract class ILTranslationConvention
    {
        public abstract void ApplyTo(ILTranslator translator);
    }

}
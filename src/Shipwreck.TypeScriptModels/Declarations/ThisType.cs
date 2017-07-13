using System.IO;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public sealed class ThisType : ITypeReference
    {
        public bool? IsClass => true;

        public bool? IsInterface => false;

        public bool? IsEnum => false;

        public void WriteTypeReference(TextWriter writer)
            => writer.Write("this");
    }
}
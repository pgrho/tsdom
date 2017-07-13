using System.IO;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public sealed class TypeQuery : ITypeReference
    {
        public bool? IsClass => null;

        public bool? IsInterface => null;

        public bool? IsEnum => null;

        public string Name { get; set; }

        public void WriteTypeReference(TextWriter writer)
        {
            writer.Write("typeof ");
            writer.Write(Name);
        }
    }
}
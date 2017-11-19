using System.IO;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public sealed class StringLiteralType : ITypeReference
    {
        public string Name => "string";
        public bool? IsClass => false;

        public bool? IsInterface => false;

        public bool? IsEnum => false;

        public string Value { get; set; }

        public void WriteTypeReference(TextWriter writer)
            => writer.WriteLiteral(Value);
    }
}
using System.IO;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public sealed class ArrayType : ITypeReference
    {
        public ArrayType()
        {
        }

        public ArrayType(ITypeReference elementType)
        {
            ElementType = elementType;
        }

        public string Name => "Array";

        public bool? IsClass => true;

        public bool? IsInterface => false;

        public bool? IsEnum => false;

        public bool? IsPrimitive => false;

        public ITypeReference ElementType { get; set; }

        public void WriteTypeReference(TextWriter writer)
        {
            if (ElementType != null)
            {
                ElementType.WriteTypeReference(writer);
            }
            writer.Write("[]");
        }
    }
}
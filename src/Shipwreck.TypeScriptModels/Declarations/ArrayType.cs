using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public sealed class ArrayType : ITypeReference
    {
        public string Name => "Array";

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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public sealed class PropertySignature : Signature
    {
        public string PropertyName { get; set; }
        public bool IsOptional { get; set; }
        public ITypeReference PropertyType { get; set; }

        internal override void WriteSignature(TextWriter writer)
        {
            writer.Write(PropertyName);
            if (IsOptional)
            {
                writer.Write('?');
            }
            if (PropertyType != null)
            {
                writer.Write(": ");
                PropertyType.WriteTypeReference(writer);
            }
        }
    }
}

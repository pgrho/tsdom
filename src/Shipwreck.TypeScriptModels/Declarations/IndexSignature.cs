using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public sealed class IndexSignature : Signature
    {
        public string ParameterName { get; set; }
        public bool IsNumeric { get; set; }
        public ITypeReference ReturnType { get; set; }

        internal override void WriteSignature(TextWriter writer)
        {
            writer.Write('[');
            writer.Write(ParameterName);
            writer.Write(IsNumeric ? ": number]: " : ": string]: ");
            (ReturnType ?? PredefinedType.Any).WriteTypeReference(writer);
        }
    }
}

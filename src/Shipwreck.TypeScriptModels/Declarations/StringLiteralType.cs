using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public sealed class StringLiteralType : ITypeReference
    {
        public string Name => "string";

        public string Value { get; set; }

        public void WriteTypeReference(TextWriter writer)
            => writer.WriteLiteral(Name);
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public sealed class TypeQuery : ITypeReference
    {
        public string Name { get; set; }

        public void WriteTypeReference(TextWriter writer)
        {
            writer.Write("typeof ");
            writer.Write(Name);
        }
    }
}

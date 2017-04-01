using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public sealed class ThisType : ITypeReference
    {
        public void WriteTypeReference(TextWriter writer)
            => writer.Write("this");
    }
}

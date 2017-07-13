using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels
{
    public interface ITypeReference
    {
        bool? IsClass { get; }
        bool? IsInterface { get; }
        bool? IsEnum { get; }

        void WriteTypeReference(TextWriter writer);
    }
}

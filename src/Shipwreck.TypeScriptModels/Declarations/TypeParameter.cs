using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public sealed class TypeParameter
    {
        public string Name { get; set; }

        public ITypeReference Constraint { get; set; }
    }
}

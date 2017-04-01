using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public sealed class Parameter
    {
        public AccessibilityModifier Accessibility { get; set; }
        public string ParameterName { get; set; }
        public ITypeReference ParameterType { get; set; }
        public bool IsOptional { get; set; }
        public bool IsRest { get; set; }
    }
}

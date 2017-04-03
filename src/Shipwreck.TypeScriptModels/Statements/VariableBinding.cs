using System.ComponentModel;

namespace Shipwreck.TypeScriptModels.Statements
{

    public sealed class VariableBinding
    {
        [DefaultValue(null)]
        public Expression Variable { get; set; }

        [DefaultValue(null)]
        public ITypeReference Type { get; set; }

        [DefaultValue(null)]
        public Expression Initializer { get; set; }
    }
}
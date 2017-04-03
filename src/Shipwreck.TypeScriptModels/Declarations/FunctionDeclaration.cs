using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Shipwreck.TypeScriptModels.Declarations
{
    // 6.1
    public sealed class FunctionDeclaration : FunctionDeclarationBase
    {
        /// <summary>
        /// Gets or sets the value indicating whether the function has a <c>declare</c> modifier.
        /// </summary>
        [DefaultValue(false)]
        public bool IsDeclare { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether the function has a <c>export</c> modifier.
        /// </summary>
        [DefaultValue(false)]
        public bool IsExport { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether the function has a <c>default</c> modifier.
        /// </summary>
        [DefaultValue(false)]
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets the binding identifier of this function.
        /// </summary>
        [DefaultValue(null)]
        public string FunctionName { get; set; }

    }
}
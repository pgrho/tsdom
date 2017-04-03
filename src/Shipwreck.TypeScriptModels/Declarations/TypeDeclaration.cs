using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public abstract class TypeDeclaration : ITypeReference
    {
        /// <summary>
        /// Gets or sets the value indicating whether the type has a <c>declare</c> modifier.
        /// </summary>
        [DefaultValue(false)]
        public bool IsDeclare { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether the type has a <c>export</c> modifier.
        /// </summary>
        [DefaultValue(false)]
        public bool IsExport { get; set; }

        public string Name { get; set; }

        public virtual void WriteTypeReference(TextWriter writer)
            => writer.Write(Name);
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public sealed class NamedTypeReference : ITypeReference
    {
        public string Name { get; set; }

        public bool? IsClass { get; set; }
        public bool? IsInterface { get; set; }

        public bool? IsEnum { get; set; }

        public bool? IsPrimitive { get; set; }

        private Collection<ITypeReference> _TypeArguments;

        public bool HasTypeArgument
            => _TypeArguments?.Count > 0;

        public Collection<ITypeReference> TypeArguments
        {
            get
            {
                return CollectionHelper.GetOrCreate(ref _TypeArguments);
            }
            set
            {
                CollectionHelper.Set(ref _TypeArguments, value);
            }
        }
        public void WriteTypeReference(TextWriter writer)
        {
            writer.Write(Name);
            if (HasTypeArgument)
            {
                writer.Write('<');

                for (var i = 0; i < TypeArguments.Count; i++)
                {
                    if (i > 0)
                    {
                        writer.Write(", ");
                    }
                    TypeArguments[i].WriteTypeReference(writer);
                }

                writer.Write('>');
            }
        }
    }
}
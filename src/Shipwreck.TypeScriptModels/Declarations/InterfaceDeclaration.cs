using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public sealed class InterfaceDeclaration : TypeDeclaration
    {
        private Collection<TypeParameter> _TypeParameters;

        public bool HasTypeParameter
            => _TypeParameters?.Count > 0;

        public Collection<TypeParameter> TypeParameters
        {
            get
            {
                return CollectionHelper.GetOrCreate(ref _TypeParameters);
            }
            set
            {
                CollectionHelper.Set(ref _TypeParameters, value);
            }
        }

        public override void WriteTypeReference(TextWriter writer)
        {
            writer.Write(Name);
            writer.WriteTypeParameters(_TypeParameters);
        }
    }
}

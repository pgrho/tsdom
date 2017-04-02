﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public sealed class TypeAliasDeclaration : ITypeReference
    {
        public string Name { get; set; }

        public bool? IsPrimitive { get; set; }

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

        public void WriteTypeReference(TextWriter writer)
        {
            writer.Write(Name);
            if (HasTypeParameter)
            {
                writer.Write('<');

                for (var i = 0; i < TypeParameters.Count; i++)
                {
                    if (i > 0)
                    {
                        writer.Write(", ");
                    }
                    writer.Write(TypeParameters[i].Name);
                }

                writer.Write('>');
            }
        }

        public ITypeReference Type { get; set; }
    }
}
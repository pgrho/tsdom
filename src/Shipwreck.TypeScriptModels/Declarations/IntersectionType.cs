﻿using System.Collections.ObjectModel;
using System.IO;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public sealed class IntersectionType : ITypeReference
    {
        public bool? IsClass => false;

        public bool? IsInterface => true;

        public bool? IsEnum => false;

        private Collection<ITypeReference> _ElementTypes;

        public Collection<ITypeReference> ElementTypes
        {
            get
            {
                return CollectionHelper.GetOrCreate(ref _ElementTypes);
            }
            set
            {
                CollectionHelper.Set(ref _ElementTypes, value);
            }
        }

        public void WriteTypeReference(TextWriter writer)
        {
            for (var i = 0; i < ElementTypes.Count; i++)
            {
                if (i > 0)
                {
                    writer.Write(" & ");
                }
                writer.Write('(');
                ElementTypes[i].WriteTypeReference(writer);
                writer.Write(')');
            }
        }
    }
}
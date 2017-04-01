using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public sealed class IntersectionType : ITypeReference
    {
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
                ElementTypes[i].WriteTypeReference(writer);
            }
        }
    }
}
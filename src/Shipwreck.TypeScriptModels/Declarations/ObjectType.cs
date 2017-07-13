using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipwreck.TypeScriptModels.Declarations;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public sealed class ObjectType : ITypeReference
    {
        public bool? IsClass => false;

        public bool? IsInterface => true ;

        public bool? IsEnum => false;

        private Collection<Signature> _Members;

        public bool HasMember
            => _Members?.Count > 0;

        public Collection<Signature> Members
        {
            get
            {
                return CollectionHelper.GetOrCreate(ref _Members);
            }
            set
            {
                CollectionHelper.Set(ref _Members, value);
            }
        }

        public void WriteTypeReference(TextWriter writer)
        {
            writer.Write('{');
            if (_Members?.Count > 0)
            {
                for (var i = 0; i < _Members.Count; i++)
                {
                    if (i > 0)
                    {
                        writer.Write(", ");
                    }

                    _Members[i].WriteSignature(writer);
                }

            }
            writer.Write('}');
        }
    }
}

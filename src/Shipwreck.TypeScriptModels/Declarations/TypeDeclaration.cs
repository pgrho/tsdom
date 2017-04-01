using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public abstract class TypeDeclaration : ITypeReference
    {
        public string Name { get; set; }
         
        public virtual void WriteTypeReference(TextWriter writer)
            => writer.Write(Name);
    }



}

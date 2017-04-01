using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Shipwreck.TypeScriptModels
{
    public sealed class ArrayType : ITypeScriptType
    {
        public ArrayType(ITypeScriptType elementType)
        {
            ElementType = elementType;
        }

        public ITypeScriptType ElementType { get; }

        string ITypeScriptType.Name
            => ElementType.Name + "[]";

        public void WriteTypeName(TextWriter writer)
        {
            ElementType.WriteTypeName(writer);
            writer.Write("[]");
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Shipwreck.TypeScriptModels
{
    public sealed class UnknownType : ITypeScriptType
    {
        public UnknownType(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public void WriteTypeName(TextWriter writer)
            => writer.Write(Name);
    }
}
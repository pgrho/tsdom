using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Shipwreck.TypeScriptModels
{
    public sealed class BuiltinType : ITypeScriptType
    {
        public static readonly BuiltinType Any = new BuiltinType("any");
        public static readonly BuiltinType Number = new BuiltinType("number");
        public static readonly BuiltinType String = new BuiltinType("string");
        public static readonly BuiltinType Boolean = new BuiltinType("boolean");
        public static readonly BuiltinType Function = new BuiltinType("function");
        public static readonly BuiltinType Error = new BuiltinType("Error");
        public static readonly BuiltinType Object = new BuiltinType("Object");

        private BuiltinType(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public void WriteTypeName(TextWriter writer)
            => writer.Write(Name);
    }
}
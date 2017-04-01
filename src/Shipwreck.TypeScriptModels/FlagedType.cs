using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Shipwreck.TypeScriptModels
{
    public struct FlagedType
    {
        public FlagedType(ITypeScriptType type, TypeFlags flags = TypeFlags.None)
        {
            Type = type;
            Flags = flags;
        }

        public ITypeScriptType Type { get; }
        public TypeFlags Flags { get; }

        public bool IsRequired => (Flags | TypeFlags.Required) != 0;
        public bool IsNullable => (Flags | TypeFlags.Nullable) != 0;
    }
}
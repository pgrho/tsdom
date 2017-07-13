using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public sealed class PredefinedType : ITypeReference
    {
        public static readonly PredefinedType Any = new PredefinedType("any", false);
        public static readonly PredefinedType Boolean = new PredefinedType("boolean", true);
        public static readonly PredefinedType String = new PredefinedType("string", true);
        public static readonly PredefinedType Number = new PredefinedType("number", true);
        public static readonly PredefinedType Symbol = new PredefinedType("symbol", true);
        public static readonly PredefinedType Void = new PredefinedType("void", true);

        private PredefinedType(string name, bool primitive)
        {
            Name = name;
            IsPrimitive = primitive;
        }

        public bool? IsClass => false;

        public bool? IsInterface => false;

        public bool? IsEnum => false;

        public string Name { get; }

        public bool IsPrimitive { get; }

        public void WriteTypeReference(TextWriter writer)
            => writer.Write(Name);
    }
}

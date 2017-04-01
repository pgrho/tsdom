using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Shipwreck.TypeScriptModels
{
    public class FunctionType : ITypeScriptType
    {
        public FunctionType(IEnumerable<FlagedType> underlyingTypes)
        {
            ParameterTypes = underlyingTypes.ToArray();
        }

        public string Name => "function(" + string.Join(", ", ParameterTypes.Select(_ => _.Type.Name + (_.IsNullable ? "?" : ""))) + ")";

        public FlagedType[] ParameterTypes { get; }

        public void WriteTypeName(TextWriter writer)
        {
            writer.Write('(');

            for (var i = 0; i < ParameterTypes.Length; i++)
            {
                if (i > 0)
                {
                    writer.Write(", ");
                }

                var pt = ParameterTypes[i];
                writer.Write("__arg");
                writer.Write(i);
                if (pt.IsNullable)
                {
                    writer.Write('?');
                }

                writer.Write(": ");
                pt.Type.WriteTypeName(writer);
            }

            writer.Write(") => ");
            writer.Write("any");
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Shipwreck.TypeScriptModels
{
    public class UnionType : ITypeScriptType
    {
        public UnionType(IEnumerable<ITypeScriptType> underlyingTypes)
        {
            UnderlyingTypes = underlyingTypes.ToArray();
        }

        public string Name => string.Join(" | ", UnderlyingTypes.Select(_ => _.Name));

        public ITypeScriptType[] UnderlyingTypes { get; }

        public void WriteTypeName(TextWriter writer)
        {
            for (var i = 0; i < UnderlyingTypes.Length; i++)
            {
                if (i > 0)
                {
                    writer.Write(" | ");
                }
                UnderlyingTypes[i].WriteTypeName(writer);
            }
        }
    }
}
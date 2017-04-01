using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Shipwreck.TypeScriptModels
{
    public class PromiseType : ITypeScriptType
    {
        public PromiseType(ITypeScriptType resultType)
        {
            ResultType = resultType;
        }

        public string Name => "Promise<" + ResultType.Name + ">";

        public ITypeScriptType ResultType { get; }

        public void WriteTypeName(TextWriter writer)
        {
            writer.Write("Promise<");
            ResultType.WriteTypeName(writer);
            writer.Write('>');
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Shipwreck.TypeScriptModels
{
    public interface ITypeScriptType
    {
        string Name { get; }

        void WriteTypeName(TextWriter writer);
    }
}
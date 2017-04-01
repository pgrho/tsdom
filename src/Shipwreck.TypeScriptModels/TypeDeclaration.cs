using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Shipwreck.TypeScriptModels
{
    public abstract class TypeDeclaration : Declaration, ITypeScriptType
    {
        public bool IsExport { get; set; }

        public ModuleDeclaration Module
            => Owner as ModuleDeclaration;

        public void WriteTypeName(TextWriter writer)
        {
            if (Module != null)
            {
                writer.Write(Module.Name);
                writer.Write('.');
            }
            writer.Write(Name);
        }
    }
}
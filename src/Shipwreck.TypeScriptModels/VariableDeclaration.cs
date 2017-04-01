using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Shipwreck.TypeScriptModels
{
    public sealed class VariableDeclaration : Declaration
    {
        public bool IsConstant { get; set; }

        public ITypeScriptType VariableType { get; set; }

        public override void WriteAsDeclaration(IndentedTextWriter writer)
        {
            writer.WriteDocumentation(Documentation);
            writer.Write("var ");
            writer.Write(Name);
            if (VariableType != null)
            {
                writer.Write(": ");
                VariableType.WriteTypeName(writer);
            }
            writer.WriteLine(";");
        }
    }
}
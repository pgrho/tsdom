using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Shipwreck.TypeScriptModels
{
    public sealed class PropertyDeclaration : MemberDeclaration
    {
        public bool IsRequired { get; set; }

        public ITypeScriptType PropertyType { get; set; }

        public override void WriteAsDeclaration(IndentedTextWriter writer)
        {
            if (AccessModifier != AccessModifier.Public)
            {
                return;
            }

            writer.WriteDocumentation(Documentation);
            writer.Write(Name);
            if (!IsRequired)
            {
                writer.Write('?');
            }
            if (PropertyType != null)
            {
                writer.Write(": ");
                PropertyType.WriteTypeName(writer);
            }
            writer.WriteLine(";");
        }
    }
}
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Shipwreck.TypeScriptModels
{
    public sealed class FieldDeclaration : MemberDeclaration
    {
        public ITypeScriptType FieldType { get; set; }

        public override void WriteAsDeclaration(IndentedTextWriter writer)
        {
            if (AccessModifier != AccessModifier.Public)
            {
                return;
            }
            writer.WriteDocumentation(Documentation);

            writer.Write(Name);
            //if (!IsRequired)
            //{
            //    writer.Write('?');
            //}
            if (FieldType != null)
            {
                writer.Write(": ");
                FieldType.WriteTypeName(writer);
            }
            writer.WriteLine(";");
        }
    }
}
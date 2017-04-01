using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Shipwreck.TypeScriptModels
{
    public class EnumDeclaration : TypeDeclaration
    {
        private TypeScriptCollection<FieldDeclaration> _Members;

        public TypeScriptCollection<FieldDeclaration> Members
        {
            get
            {
                return _Members ?? (_Members = new TypeScriptCollection<FieldDeclaration>());
            }
            set
            {
                _Members?.Clear();
                if (value?.Count > 0)
                {
                    Members.AddRange(value);
                }
            }
        }

        public override void WriteAsDeclaration(IndentedTextWriter writer)
        {
            writer.WriteDocumentation(Documentation);
            writer.Write("enum ");
            writer.Write(Name);
            writer.WriteLine(" {");

            writer.Indent++;
            if (_Members != null)
            {
                foreach (var s in _Members)
                {
                    writer.WriteDocumentation(s.Documentation);
                    writer.Write(s.Name);
                    writer.WriteLine(",");
                }
            }
            writer.Indent--;

            writer.WriteLine("}");
        }
    }
}
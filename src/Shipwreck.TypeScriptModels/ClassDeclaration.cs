using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Shipwreck.TypeScriptModels
{
    public class ClassDeclaration : TypeDeclaration, ITypeScriptObjectOwner
    {
        private TypeScriptCollection<MemberDeclaration> _Members;

        public bool IsAbstract { get; set; }

        public TypeScriptCollection<MemberDeclaration> Members
        {
            get
            {
                return _Members ?? (_Members = new TypeScriptCollection<MemberDeclaration>(this));
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

            writer.Write("interface ");
            writer.Write(Name);
            writer.WriteLine(" {");

            writer.Indent++;
            if (_Members != null)
            {
                foreach (var s in _Members)
                {
                    s.WriteAsDeclaration(writer);
                }
            }
            writer.Indent--;

            writer.WriteLine("}");

            writer.Write("var ");
            writer.Write(Name);
            writer.Write(": ");
            writer.Write(Name);
            writer.WriteLine(";");
        }
    }
}
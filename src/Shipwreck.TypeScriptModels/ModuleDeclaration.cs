using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace Shipwreck.TypeScriptModels
{
    public class ModuleDeclaration : Declaration
    {
        private TypeScriptCollection<Statement> _Statements;

        public TypeScriptCollection<Statement> Statements
        {
            get
            {
                return _Statements ?? (_Statements = new TypeScriptCollection<Statement>());
            }
            set
            {
                _Statements?.Clear();
                if (value?.Count > 0)
                {
                    Statements.AddRange(value);
                }
            }
        }
        public override void WriteAsDeclaration(IndentedTextWriter writer)
        {
            writer.WriteDocumentation(Documentation);
            writer.Write("declare namespace ");
            writer.Write(Name);
            writer.WriteLine(" {");

            writer.Indent++;
            if (_Statements != null)
            {
                foreach (var s in _Statements)
                {
                    s.WriteAsDeclaration(writer);
                }
            }
            writer.Indent--;

            writer.WriteLine("}");
        }
    }
}
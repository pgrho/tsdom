using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipwreck.TypeScriptModels.Declarations;

namespace Shipwreck.TypeScriptModels.Expressions
{
    public interface IObjectLiteralMember
    {
        void Accept<T>(IObjectLiteralVisitor<T> visitor);
    }
}

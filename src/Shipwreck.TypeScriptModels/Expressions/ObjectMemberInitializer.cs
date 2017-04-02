using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Expressions
{
    public sealed class ObjectMemberInitializer : IObjectLiteralMember
    {
        public string PropertyName { get; set; }
        public Expression Value { get; set; }

        public void Accept<T>(IObjectLiteralVisitor<T> visitor)
            => visitor.VisitMemberInitializer(this);
    }
}
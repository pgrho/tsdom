using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Expressions
{
    public sealed class SuperExpression : Expression
    {
        public override void Accept<T>(IExpressionVisitor<T> visitor) 
            => visitor.VisitSuper();
    }
}

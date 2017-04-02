using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Expressions
{
    public sealed class ConditionalExpression : Expression
    {
        public Expression Condition { get; set; }

        public Expression TruePart { get; set; }

        public Expression FalsePart { get; set; }

        public override void Accept<T>(IExpressionVisitor<T> visitor)
            => visitor.VisitConditional(this);
    }
}

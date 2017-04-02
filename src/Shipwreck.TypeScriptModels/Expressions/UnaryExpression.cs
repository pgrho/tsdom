using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Expressions
{
    public sealed class UnaryExpression : Expression
    {
        public UnaryOperator Operator { get; set; }
        public Expression Operand { get; set; }

        public override void Accept<T>(IExpressionVisitor<T> visitor)
            => visitor.VisitUnary(this);
    }
}

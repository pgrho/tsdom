using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Expressions
{
    public sealed class BinaryExpression : Expression
    {
        public BinaryOperator Operator { get; set; }
        public Expression Left { get; set; }
        public Expression Right { get; set; }

        public override void Accept<T>(IExpressionVisitor<T> visitor)
            => visitor.VisitBinary(this);
    }
}

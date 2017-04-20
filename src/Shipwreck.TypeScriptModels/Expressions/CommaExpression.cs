using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Expressions
{
    // 4.22
    public sealed class CommaExpression : Expression
    {
        public override ExpressionPrecedence Precedence
            => ExpressionPrecedence.Comma;

        public Expression Left { get; set; }
        public Expression Right { get; set; }

        public override void Accept<T>(IExpressionVisitor<T> visitor)
            => visitor.VisitComma(this);
    }
}

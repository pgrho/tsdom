using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Expressions
{
    public sealed class ParenthesesExpression : Expression
    {
        public override ExpressionPrecedence Precedence
            => ExpressionPrecedence.Grouping;

        public Expression Expression { get; set; }

        public override void Accept<T>(IExpressionVisitor<T> visitor)
            => visitor.VisitParentheses(this);
    }
}

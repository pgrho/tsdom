using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Expressions
{
    public sealed class ForBindingExpression : Expression
    {
        public override ExpressionPrecedence Precedence
            => ExpressionPrecedence.Grouping;

        public Expression Variable { get; }
        public Expression Value { get; }

        public override void Accept<T>(IExpressionVisitor<T> visitor)
        {
            throw new NotSupportedException();
        }
    }
}

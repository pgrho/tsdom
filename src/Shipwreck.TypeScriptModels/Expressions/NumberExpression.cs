using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Expressions
{
    // 4.4
    public sealed class NumberExpression : Expression
    {
        public double Value { get; set; }

        public override void Accept<T>(IExpressionVisitor<T> visitor)
            => visitor.VisitNumber(this);
    }
}

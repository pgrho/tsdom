using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Expressions
{
    public sealed class FunctionExpression : FunctionExpressionBase
    {
        public string FunctionName { get; set; }

        public override void Accept<T>(IExpressionVisitor<T> visitor)
            => visitor.VisitFunction(this);
    }
}

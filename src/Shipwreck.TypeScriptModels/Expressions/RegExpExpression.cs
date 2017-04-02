using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Expressions
{
    // 4.4
    public sealed class RegExpExpression : Expression
    {
        public string Pattern { get; set; }

        public string Option { get; set; }

        public override void Accept<T>(IExpressionVisitor<T> visitor)
            => visitor.VisitRegExp(this);
    }
}

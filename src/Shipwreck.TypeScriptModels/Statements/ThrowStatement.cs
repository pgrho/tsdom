using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Statements
{
    // 5.13
    public sealed class ThrowStatement : Statement
    {
        public Expression Value { get; set; }

        public override T Accept<T>(IStatementVisitor<T> visitor)
            => visitor.VisitThrow(this);
    }
}
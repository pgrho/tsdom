using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Statements
{
    // 5.11
    public sealed class WithStatement : Statement
    {
        public override T Accept<T>(IStatementVisitor<T> visitor)
            => visitor.VisitWith(this);
    }
}
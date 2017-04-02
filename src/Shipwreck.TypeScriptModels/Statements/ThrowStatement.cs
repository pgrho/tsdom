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
        public override T Accept<T>(IStatementVistor<T> visitor)
            => visitor.VisitThrow(this);
    }
}

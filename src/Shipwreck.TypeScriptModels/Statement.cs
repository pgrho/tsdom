using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels
{
    public abstract class Statement : Syntax
    {
        public abstract T Accept<T>(IStatementVisitor<T> visitor);
    }
}

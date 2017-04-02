using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Expressions
{
    // 4.2
    public sealed class ThisExpression : Expression
    {
        public override void Accept<T>(IExpressionVisitor<T> visitor)
            => visitor.VisitThis();
    }
}
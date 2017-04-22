using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Expressions
{ 
    public sealed class IndexerExpression : Expression
    {
        public override ExpressionPrecedence Precedence
            => ExpressionPrecedence.MemberAccess;

        public Expression Object { get; set; }

        public Expression Index { get; set; }
         
        public override void Accept<T>(IExpressionVisitor<T> visitor)
            => visitor.VisitIndexer(this);
    }
}

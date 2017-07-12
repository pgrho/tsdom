using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Expressions
{
    // 4.3
    public sealed class IdentifierExpression : Expression
    {
        public IdentifierExpression()
        {
        }

        public IdentifierExpression(string name)
        {
            Name = name;
        }

        public override ExpressionPrecedence Precedence
            => ExpressionPrecedence.Grouping;

        public string Name { get; set; }

        public override void Accept<T>(IExpressionVisitor<T> visitor)
            => visitor.VisitIdentifier(this);

        // TODO: property
        // TODO: variable
        // TODO: constructor
    }
}
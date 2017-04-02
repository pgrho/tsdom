using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Expressions
{
    public sealed class AssignmentExpression : Expression
    {
        public Expression Target { get; set; }

        public Expression Value { get; set; }

        public BinaryOperator CompoundOperator { get; set; }

        public override void Accept<T>(IExpressionVisitor<T> visitor)
            => visitor.VisitAssignment(this);

        public bool IsDestruturing
        {
            get
            {
                // TODO: Implement for JavaScript Generation
                throw new Exception();
            }
        }
    }
}

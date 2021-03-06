using System.ComponentModel;

namespace Shipwreck.TypeScriptModels.Statements
{
    public sealed class ExpressionStatement : Statement
    {
        public ExpressionStatement()
        {
        }

        public ExpressionStatement(Expression expression)
        {
            Expression = expression;
        }

        [DefaultValue(null)]
        public Expression Expression { get; set; }

        public override T Accept<T>(IStatementVisitor<T> visitor)
            => visitor.VisitExpression(this);
    }
}
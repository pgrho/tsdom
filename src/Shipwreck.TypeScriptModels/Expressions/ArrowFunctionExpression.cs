using Shipwreck.TypeScriptModels.Declarations;

namespace Shipwreck.TypeScriptModels.Expressions
{
    public sealed class ArrowFunctionExpression : FunctionExpressionBase
    {
        public ArrowFunctionExpression()
        {
        }

        public ArrowFunctionExpression(Expression expression)
        {
            Statements.Add(expression.ToReturn());
        }

        public ArrowFunctionExpression(Parameter parameter, Expression expression)
        {
            Parameters.Add(parameter);
            Statements.Add(expression.ToReturn());
        }

        public override void Accept<T>(IExpressionVisitor<T> visitor)
            => visitor.VisitArrowFunction(this);
    }
}
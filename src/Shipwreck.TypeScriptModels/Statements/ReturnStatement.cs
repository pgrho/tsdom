namespace Shipwreck.TypeScriptModels.Statements
{
    // 5.10
    public sealed class ReturnStatement : Statement
    {
        public Expression Value { get; set; }

        public override T Accept<T>(IStatementVisitor<T> visitor)
            => visitor.VisitReturn(this);
    }
}
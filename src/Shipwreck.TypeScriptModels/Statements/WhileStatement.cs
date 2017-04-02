namespace Shipwreck.TypeScriptModels.Statements
{
    // 5.4
    public sealed class WhileStatement : Statement
    {
        public override T Accept<T>(IStatementVistor<T> visitor)
            => visitor.VisitWhile(this);
    }
}
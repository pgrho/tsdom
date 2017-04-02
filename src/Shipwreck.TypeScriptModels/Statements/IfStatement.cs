namespace Shipwreck.TypeScriptModels.Statements
{

    // 5.4
    public sealed class IfStatement : Statement
    {
        public override T Accept<T>(IStatementVistor<T> visitor)
            => visitor.VisitIf(this);
    }
}
namespace Shipwreck.TypeScriptModels.Statements
{

    // 5.4
    public sealed class DoStatement : Statement
    {
        public override T Accept<T>(IStatementVistor<T> visitor)
            => visitor.VisitDo(this);
    }
}
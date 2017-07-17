namespace Shipwreck.TypeScriptModels.Statements
{
    public sealed class SwitchCase : Syntax, IStatementOwner
    {
        public Expression Label { get; set; }

        private StatementCollection _Statements;

        public bool HasStatement
            => _Statements?.Count > 0;

        public StatementCollection Statements
        {
            get
            {
                return CollectionHelper.GetOrCreate(ref _Statements, this);
            }
            set
            {
                CollectionHelper.Set(ref _Statements, value, this);
            }
        }
    }
}
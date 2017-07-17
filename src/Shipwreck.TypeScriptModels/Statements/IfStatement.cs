namespace Shipwreck.TypeScriptModels.Statements
{
    // 5.4
    public sealed class IfStatement : Statement, IStatementOwner
    {
        public Expression Condition { get; set; }

        private StatementCollection _TruePart;

        public bool HasTruePart
            => _TruePart?.Count > 0;

        public StatementCollection TruePart
        {
            get
            {
                return CollectionHelper.GetOrCreate(ref _TruePart, this);
            }
            set
            {
                CollectionHelper.Set(ref _TruePart, value, this);
            }
        }

        private StatementCollection _FalsePart;

        public bool HasFalsePart
            => _FalsePart?.Count > 0;

        public StatementCollection FalsePart
        {
            get
            {
                return CollectionHelper.GetOrCreate(ref _FalsePart, this);
            }
            set
            {
                CollectionHelper.Set(ref _FalsePart, value, this);
            }
        }

        public override T Accept<T>(IStatementVisitor<T> visitor)
            => visitor.VisitIf(this);
    }
}
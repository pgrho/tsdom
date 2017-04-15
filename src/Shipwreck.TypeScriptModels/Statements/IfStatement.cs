using System.Collections.ObjectModel;

namespace Shipwreck.TypeScriptModels.Statements
{
    // 5.4
    public sealed class IfStatement : Statement
    {
        public Expression Condition { get; set; }

        private Collection<Statement> _TruePart;

        public bool HasTruePart
            => _TruePart?.Count > 0;

        public Collection<Statement> TruePart
        {
            get
            {
                return CollectionHelper.GetOrCreate(ref _TruePart);
            }
            set
            {
                CollectionHelper.Set(ref _TruePart, value);
            }
        }

        private Collection<Statement> _FalsePart;

        public bool HasFalsePart
            => _FalsePart?.Count > 0;

        public Collection<Statement> FalsePart
        {
            get
            {
                return CollectionHelper.GetOrCreate(ref _FalsePart);
            }
            set
            {
                CollectionHelper.Set(ref _FalsePart, value);
            }
        }

        public override T Accept<T>(IStatementVisitor<T> visitor)
            => visitor.VisitIf(this);
    }
}
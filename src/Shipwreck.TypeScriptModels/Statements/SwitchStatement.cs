using System.Collections.ObjectModel;

namespace Shipwreck.TypeScriptModels.Statements
{
    // 5.12
    public sealed class SwitchStatement : Statement
    {
        private Collection<SwitchCase> _Cases;

        public Expression Condition { get; set; }

        public bool HasCase
            => _Cases?.Count > 0;

        public Collection<SwitchCase> Cases
        {
            get
            {
                return CollectionHelper.GetOrCreate(ref _Cases);
            }
            set
            {
                CollectionHelper.Set(ref _Cases, value);
            }
        }

        public override T Accept<T>(IStatementVisitor<T> visitor)
            => visitor.VisitSwitch(this);
    }
}
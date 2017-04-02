using System.Collections.ObjectModel;

namespace Shipwreck.TypeScriptModels.Statements
{
    public sealed class SwitchCase
    {
        public Expression Label { get; set; }

        private Collection<Statement> _Statements;

        public bool HasStatement
            => _Statements?.Count > 0;

        public Collection<Statement> Statements
        {
            get
            {
                return CollectionHelper.GetOrCreate(ref _Statements);
            }
            set
            {
                CollectionHelper.Set(ref _Statements, value);
            }
        }
    }
}
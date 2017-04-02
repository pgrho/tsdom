using System.Collections.ObjectModel;

namespace Shipwreck.TypeScriptModels.Statements
{
    // 5.2
    public sealed class VariableDeclaration : Statement
    {
        private Collection<VariableBinding> _Bindings;

        public bool HasBinding
            => _Bindings?.Count > 0;

        public Collection<VariableBinding> Bindings
        {
            get
            {
                return CollectionHelper.GetOrCreate(ref _Bindings);
            }
            set
            {
                CollectionHelper.Set(ref _Bindings, value);
            }
        }

        public override T Accept<T>(IStatementVistor<T> visitor)
            => visitor.VisitVariableDeclaration(this);
    }
}
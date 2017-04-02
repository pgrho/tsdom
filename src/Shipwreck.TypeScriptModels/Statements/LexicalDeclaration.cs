using System.Collections.ObjectModel;

namespace Shipwreck.TypeScriptModels.Statements
{

    // 5.3
    public abstract class LexicalDeclaration : Statement
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
    }
}
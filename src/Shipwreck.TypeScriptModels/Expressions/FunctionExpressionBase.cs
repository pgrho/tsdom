using Shipwreck.TypeScriptModels.Declarations;
using System.Collections.ObjectModel;

namespace Shipwreck.TypeScriptModels.Expressions
{
    public abstract class FunctionExpressionBase : Expression, ICallSignature, IStatementOwner
    {
        private Collection<TypeParameter> _TypeParameters;

        public bool HasTypeParameter
            => _TypeParameters?.Count > 0;

        public Collection<TypeParameter> TypeParameters
        {
            get
            {
                return CollectionHelper.GetOrCreate(ref _TypeParameters);
            }
            set
            {
                CollectionHelper.Set(ref _TypeParameters, value);
            }
        }

        private Collection<Parameter> _Parameters;

        public bool HasParameter
            => _Parameters?.Count > 0;

        public Collection<Parameter> Parameters
        {
            get
            {
                return CollectionHelper.GetOrCreate(ref _Parameters);
            }
            set
            {
                CollectionHelper.Set(ref _Parameters, value);
            }
        }

        public ITypeReference ReturnType { get; set; }

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
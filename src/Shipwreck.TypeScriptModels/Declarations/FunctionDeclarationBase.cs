using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public abstract class FunctionDeclarationBase : Syntax, ICallSignature
    {
        #region Overloads

        private Collection<CallSignature> _Overloads;

        /// <summary>
        /// Gets a value indicating whether the value of <see cref="Overloads" /> contains any element;
        /// </summary>
        public bool HasOverload
            => _Overloads?.Count > 0;

        /// <summary>
        /// Gets or sets the all overload declarations of the function.
        /// </summary>
        public Collection<CallSignature> Overloads
        {
            get
            {
                return CollectionHelper.GetOrCreate(ref _Overloads);
            }
            set
            {
                CollectionHelper.Set(ref _Overloads, value);
            }
        }

        /// <summary>
        /// Determines a value indicating whether the value of <see cref="Overloads" /> needs to be persisted.
        /// </summary>
        /// <returns><c>true</c> if the property should be persisted; otherwise, <c>false</c>.</returns>
        public bool ShouldSerializeOverloads()
            => HasOverload;

        /// <summary>
        /// Resets the value for <see cref="Overloads" /> of the function to the default value.
        /// </summary>
        public void ResetOverloads()
            => _Overloads?.Clear();

        #endregion Overloads

        #region TypeParameters

        private Collection<TypeParameter> _TypeParameters;

        /// <summary>
        /// Gets a value indicating whether the value of <see cref="TypeParameters" /> contains any element;
        /// </summary>
        public bool HasTypeParameter
            => _TypeParameters?.Count > 0;

        /// <summary>
        /// Gets or sets the all generic type parameter definitions of the function.
        /// </summary>
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

        /// <summary>
        /// Determines a value indicating whether the value of <see cref="TypeParameters" /> needs to be persisted.
        /// </summary>
        /// <returns><c>true</c> if the property should be persisted; otherwise, <c>false</c>.</returns>
        public bool ShouldSerializeTypeParameters()
            => HasTypeParameter;

        /// <summary>
        /// Resets the value for <see cref="TypeParameters" /> of the function to the default value.
        /// </summary>
        public void ResetTypeParameters()
            => _TypeParameters?.Clear();

        #endregion TypeParameters

        #region Parameters

        private Collection<Parameter> _Parameters;

        /// <summary>
        /// Gets a value indicating whether the value of <see cref="Parameters" /> contains any element;
        /// </summary>
        public bool HasParameter
            => _Parameters?.Count > 0;

        /// <summary>
        /// Gets or sets the all parameter definitions of the function.
        /// </summary>
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

        /// <summary>
        /// Determines a value indicating whether the value of <see cref="Parameters" /> needs to be persisted.
        /// </summary>
        /// <returns><c>true</c> if the property should be persisted; otherwise, <c>false</c>.</returns>
        public bool ShouldSerializeParameters()
            => HasParameter;

        /// <summary>
        /// Resets the value for <see cref="Parameters" /> of the function to the default value.
        /// </summary>
        public void ResetParameters()
            => _Parameters?.Clear();

        #endregion Parameters

        #region Statements

        private Collection<Statement> _Statements;

        /// <summary>
        /// Gets a value indicating whether the value of <see cref="Statements" /> contains any element;
        /// </summary>
        public bool HasStatement
            => _Statements?.Count > 0;

        /// <summary>
        /// Gets or sets the all statements of the function.
        /// </summary>
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

        /// <summary>
        /// Determines a value indicating whether the value of <see cref="Statements" /> needs to be persisted.
        /// </summary>
        /// <returns><c>true</c> if the property should be persisted; otherwise, <c>false</c>.</returns>
        public bool ShouldSerializeStatements()
            => HasStatement;

        /// <summary>
        /// Resets the value for <see cref="Statements" /> of the function to the default value.
        /// </summary>
        public void ResetStatements()
            => _Statements?.Clear();

        #endregion Statements

        /// <summary>
        /// Gets or sets the type of the value that the function returns.
        /// </summary>
        [DefaultValue(null)]
        public ITypeReference ReturnType { get; set; }
    }
}
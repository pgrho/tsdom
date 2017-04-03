using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Shipwreck.TypeScriptModels.Statements
{
    // 5.2
    // 5.3
    public sealed class VariableDeclaration : Statement
    {
        /// <summary>
        /// Gets or sets the value indicating whether the declaration has a <c>declare</c> modifier.
        /// </summary>
        [DefaultValue(false)]
        public bool IsDeclare { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether the declaration has a <c>export</c> modifier.
        /// </summary>
        [DefaultValue(false)]
        public bool IsExport { get; set; }

        /// <summary>
        /// Gets or sets the value representing the type of variable declaration.
        /// </summary>
        [DefaultValue(default(VariableDeclarationType))]
        public VariableDeclarationType Type { get; set; }

        #region Bindings

        private Collection<VariableBinding> _Bindings;

        /// <summary>
        /// Gets a value indicating whether the value of <see cref="Bindings" /> contains any element;
        /// </summary>
        public bool HasBinding
            => _Bindings?.Count > 0;

        /// <summary>
        /// Gets or sets the all variable bindings of the declaration.
        /// </summary>
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

        /// <summary>
        /// Determines a value indicating whether the value of <see cref="Bindings" /> needs to be persisted.
        /// </summary>
        /// <returns><c>true</c> if the property should be persisted; otherwise, <c>false</c>.</returns>
        public bool ShouldSerializeBindings()
            => HasBinding;

        /// <summary>
        /// Resets the value for <see cref="Bindings" /> of the declaration to the default value.
        /// </summary>
        public void ResetBindings()
            => _Bindings?.Clear();

        #endregion Bindings

        public override T Accept<T>(IStatementVistor<T> visitor)
            => visitor.VisitVariableDeclaration(this);
    }
}
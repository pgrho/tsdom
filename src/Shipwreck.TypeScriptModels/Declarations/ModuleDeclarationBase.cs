using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public abstract class ModuleDeclarationBase<TMember> : Syntax, IModuleDeclaration
        where TMember : class, IHasParent
    {
        internal ModuleDeclarationBase()
        {
        }

        /// <summary>
        /// Gets or sets the value indicating whether the module or namespace has a <c>declare</c> modifier.
        /// </summary>
        [DefaultValue(false)]
        public bool IsDeclare { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether the module or namespace has a <c>export</c> modifier.
        /// </summary>
        [DefaultValue(false)]
        public bool IsExport { get; set; }

        /// <summary>
        /// Gets of sets the identifier of the module or namespace declaration.
        /// </summary>
        [DefaultValue(null)]
        public string Name { get; set; }

        #region Members

        private OwnedCollection<TMember, IModuleDeclaration> _Members;

        /// <summary>
        /// Gets a value indicating whether the value of <see cref="Members" /> contains any element;
        /// </summary>
        public bool HasMember
            => _Members?.Count > 0;

        /// <summary>
        /// Gets or sets the all members of the module or namespace.
        /// </summary>
        public OwnedCollection<TMember, IModuleDeclaration> Members
        {
            get
            {
                return CollectionHelper.GetOrCreate(ref _Members, this);
            }
            set
            {
                CollectionHelper.Set(ref _Members, value, this);
            }
        }

        IList IModuleDeclaration.Members
            => Members;

        /// <summary>
        /// Determines a value indicating whether the value of <see cref="Members" /> needs to be persisted.
        /// </summary>
        /// <returns><c>true</c> if the property should be persisted; otherwise, <c>false</c>.</returns>
        public bool ShouldSerializMembers()
            => HasMember;

        /// <summary>
        /// Resets the value for <see cref="Members" /> of the module or namespace to the default value.
        /// </summary>
        public void ResetMembers()
            => _Members?.Clear();

        #endregion Members

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <param name="visitor">The visitor to visit this node with.</param>
        public abstract void Accept<T>(IRootStatementVisitor<T> visitor);
    }
}
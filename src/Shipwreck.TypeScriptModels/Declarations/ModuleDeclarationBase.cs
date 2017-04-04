using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public abstract class ModuleDeclarationBase<T> : Syntax, IModuleDeclaration
    {
        internal ModuleDeclarationBase()
        {
        }

        /// <summary>
        /// Gets of sets the identifier of the module or namespace declaration.
        /// </summary>
        [DefaultValue(null)]
        public string Name { get; set; }

        #region Members

        private Collection<T> _Members;

        /// <summary>
        /// Gets a value indicating whether the value of <see cref="Members" /> contains any element;
        /// </summary>
        public bool HasMember
            => _Members?.Count > 0;

        /// <summary>
        /// Gets or sets the all members of the module or namespace.
        /// </summary>
        public Collection<T> Members
        {
            get
            {
                return CollectionHelper.GetOrCreate(ref _Members);
            }
            set
            {
                CollectionHelper.Set(ref _Members, value);
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
    }
}
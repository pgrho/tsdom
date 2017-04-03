using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels.Declarations
{
    // 9.1
    public sealed class EnumDeclaration : TypeDeclaration
    {
        /// <summary>
        /// Gets or sets the value indicating whether the enum has a <c>const</c> modifier.
        /// </summary>
        [DefaultValue(false)]
        public bool IsConst { get; set; }

        #region Members

        private Collection<FieldDeclaration> _Members;

        /// <summary>
        /// Gets a value indicating whether the value of <see cref="Members" /> contains any element;
        /// </summary>
        public bool HasMember
            => _Members?.Count > 0;

        /// <summary>
        /// Gets or sets the all members of the enum.
        /// </summary>
        public Collection<FieldDeclaration> Members
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

        /// <summary>
        /// Determines a value indicating whether the value of <see cref="Members" /> needs to be persisted.
        /// </summary>
        /// <returns><c>true</c> if the property should be persisted; otherwise, <c>false</c>.</returns>
        public bool ShouldSerializMembers()
            => HasMember;

        /// <summary>
        /// Resets the value for <see cref="Members" /> of the enum to the default value.
        /// </summary>
        public void ResetMembers()
            => _Members?.Clear();

        #endregion Members
    }
}
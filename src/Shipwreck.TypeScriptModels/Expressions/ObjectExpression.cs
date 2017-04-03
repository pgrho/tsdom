using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipwreck.TypeScriptModels.Declarations;

namespace Shipwreck.TypeScriptModels.Expressions
{
    // 4.5
    public sealed class ObjectExpression : Expression
    {
        #region Members

        private Collection<IInterfaceMember> _Members;

        /// <summary>
        /// Gets a value indicating whether the value of <see cref="Members" /> contains any element;
        /// </summary>
        public bool HasMember
            => _Members?.Count > 0;

        /// <summary>
        /// Gets or sets the all members of the object.
        /// </summary>
        public Collection<IInterfaceMember> Members
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
        /// Resets the value for <see cref="Members" /> of the object to the default value.
        /// </summary>
        public void ResetMembers()
            => _Members?.Clear();

        #endregion Members

        public override void Accept<T>(IExpressionVisitor<T> visitor)
            => visitor.VisitObject(this);
    }
}
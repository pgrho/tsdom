using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipwreck.TypeScriptModels.Expressions;

namespace Shipwreck.TypeScriptModels.Declarations
{
    // 8.4.1
    public sealed class FieldDeclaration : Syntax, IClassMember, IInterfaceMember
    {
        /// <summary>
        /// Gets or sets the value representing the accessibility modifier of the field.
        /// </summary>
        [DefaultValue(AccessibilityModifier.None)]
        public AccessibilityModifier Accessibility { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether the field has a <c>static</c> modifier.
        /// </summary>
        [DefaultValue(false)]
        public bool IsStatic { get; set; }

        public string FieldName { get; set; }

        public ITypeReference FieldType { get; set; }

        public Expression Initializer { get; set; }

        #region Decorators

        private Collection<Decorator> _Decorators;

        /// <summary>
        /// Gets a value indicating whether the value of <see cref="Decorators" /> contains any element;
        /// </summary>
        public bool HasDecorator
            => _Decorators?.Count > 0;

        /// <summary>
        /// Gets or sets the all decorators of the field.
        /// </summary>
        public Collection<Decorator> Decorators
        {
            get
            {
                return CollectionHelper.GetOrCreate(ref _Decorators);
            }
            set
            {
                CollectionHelper.Set(ref _Decorators, value);
            }
        }

        /// <summary>
        /// Determines a value indicating whether the value of <see cref="Decorators" /> needs to be persisted.
        /// </summary>
        /// <returns><c>true</c> if the property should be persisted; otherwise, <c>false</c>.</returns>
        public bool ShouldSerializeDecorators()
            => HasDecorator;

        /// <summary>
        /// Resets the value for <see cref="Decorators" /> of the field to the default value.
        /// </summary>
        public void ResetDecorators()
            => _Decorators?.Clear();

        #endregion Decorators

        public void Accept<T>(IClassMemberVisitor<T> visitor)
            => visitor.VisitField(this);

        public void Accept<T>(IInterfaceMemberVisitor<T> visitor)
            => visitor.VisitField(this);
    }
}
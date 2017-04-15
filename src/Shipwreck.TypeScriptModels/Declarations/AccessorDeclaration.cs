using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipwreck.TypeScriptModels.Expressions;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public abstract class AccessorDeclaration : IClassMember, IInterfaceMember
    {
        /// <summary>
        /// Gets or sets the value representing the accessibility modifier of the property.
        /// </summary>
        [DefaultValue(AccessibilityModifier.None)]
        public AccessibilityModifier Accessibility { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether the property has a <c>static</c> modifier.
        /// </summary>
        [DefaultValue(false)]
        public bool IsStatic { get; set; }

        public string PropertyName { get; set; }

        public ITypeReference PropertyType { get; set; }

        public bool IsSet => GetIsSet();

        internal abstract bool GetIsSet();

        private Collection<Statement> _Statements;

        public bool HasStatement
            => _Statements?.Count > 0;

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


        #region Decorators

        private Collection<Decorator> _Decorators;

        /// <summary>
        /// Gets a value indicating whether the value of <see cref="Decorators" /> contains any element;
        /// </summary>
        public bool HasDecorator
            => _Decorators?.Count > 0;

        /// <summary>
        /// Gets or sets the all decorators of the accessor.
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
        /// Resets the value for <see cref="Decorators" /> of the accessor to the default value.
        /// </summary>
        public void ResetDecorators()
            => _Decorators?.Clear();

        #endregion Decorators

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <param name="visitor">The class member visitor to visit this node with.</param>
        public abstract void Accept<T>(IClassMemberVisitor<T> visitor);

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <param name="visitor">The interface member visitor to visit this node with.</param>
        public abstract void Accept<T>(IInterfaceMemberVisitor<T> visitor);
    }
}
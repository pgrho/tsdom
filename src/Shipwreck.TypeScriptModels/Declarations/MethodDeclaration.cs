using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Shipwreck.TypeScriptModels.Declarations
{
    // 8.4.2
    public sealed class MethodDeclaration : FunctionDeclarationBase, IClassMember, IInterfaceMember, ICallSignature
    {
        /// <summary>
        /// Gets or sets the value representing the accessibility modifier of the method.
        /// </summary>
        [DefaultValue(AccessibilityModifier.None)]
        public AccessibilityModifier Accessibility { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether the method has a <c>static</c> modifier.
        /// </summary>
        [DefaultValue(false)]
        public bool IsStatic { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether the method has a <c>async</c> modifier.
        /// </summary>
        [DefaultValue(false)]
        public bool IsAsync { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether the method has a <c>abstract</c> modifier.
        /// </summary>
        [DefaultValue(false)]
        public bool IsAbstract { get; set; }

        /// <summary>
        /// Gets or sets the binding identifier of this method.
        /// </summary>
        [DefaultValue(null)]
        public string MethodName { get; set; }

        #region Decorators

        private Collection<Decorator> _Decorators;

        /// <summary>
        /// Gets a value indicating whether the value of <see cref="Decorators" /> contains any element;
        /// </summary>
        public bool HasDecorator
            => _Decorators?.Count > 0;

        /// <summary>
        /// Gets or sets the all decorators of the method.
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
        /// Resets the value for <see cref="Decorators" /> of the method to the default value.
        /// </summary>
        public void ResetDecorators()
            => _Decorators?.Clear();

        #endregion Decorators

        public void Accept<T>(IClassMemberVisitor<T> visitor)
            => visitor.VisitMethod(this);

        public void Accept<T>(IInterfaceMemberVisitor<T> visitor)
            => visitor.VisitMethod(this);
    }
}
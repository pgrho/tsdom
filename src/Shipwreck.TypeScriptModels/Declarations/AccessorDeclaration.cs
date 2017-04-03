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

        public abstract void Accept<T>(IClassMemberVisitor<T> visitor);

        public abstract void Accept<T>(IInterfaceMemberVisitor<T> visitor);
    }
}
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipwreck.TypeScriptModels.Expressions;

namespace Shipwreck.TypeScriptModels.Declarations
{
    // 8.4.1
    public sealed class FieldDeclaration : IClassMember, IInterfaceMember
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

        public void Accept<T>(IClassMemberVisitor<T> visitor)
            => visitor.VisitField(this);

        public void Accept<T>(IInterfaceMemberVisitor<T> visitor)
            => visitor.VisitField(this);
    }
}
using System.ComponentModel;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public sealed class ConstructorDeclaration : FunctionDeclarationBase, IClassMember
    {
        /// <summary>
        /// Gets or sets the value representing the accessibility modifier of the constructor.
        /// </summary>
        [DefaultValue(AccessibilityModifier.None)]
        public AccessibilityModifier Accessibility { get; set; }

        public void Accept<T>(IClassMemberVisitor<T> visitor)
            => visitor.VisitConstructor(this);
    }
}
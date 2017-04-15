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
    public sealed class EnumDeclaration : TypeDeclaration<FieldDeclaration>
    {
        /// <summary>
        /// Gets or sets the value indicating whether the enum has a <c>const</c> modifier.
        /// </summary>
        [DefaultValue(false)]
        public bool IsConst { get; set; }

        /// <inheritdoc />
        public override void Accept<T>(IRootStatementVisitor<T> visitor)
            => visitor.VisitEnumDeclaration(this);

        /// <inheritdoc />
        public override void Accept<T>(IModuleMemberVisitor<T> visitor)
            => visitor.VisitEnumDeclaration(this);

        /// <inheritdoc />
        public override void Accept<T>(INamespaceMemberVisitor<T> visitor)
            => visitor.VisitEnumDeclaration(this);
    }
}
using System;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public sealed class ModuleDeclaration : ModuleDeclarationBase<IModuleMember>
    {
        /// <inheritdoc />
        public override void Accept<T>(IRootStatementVisitor<T> visitor)
            => visitor.VisitModuleDeclaration(this);
    }
}
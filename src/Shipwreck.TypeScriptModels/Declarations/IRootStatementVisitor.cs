namespace Shipwreck.TypeScriptModels.Declarations
{

    /// <summary>
    /// Represents a visitor for root statements that returns a value of <see cref="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the return value.</typeparam>
    public interface IRootStatementVisitor<T>
    {
        T VisitModuleDeclaration(ModuleDeclaration declaration);

        T VisitNamespaceDeclaration(NamespaceDeclaration declaration);

        T VisitClassDeclaration(ClassDeclaration declaration);

        T VisitInterfaceDeclaration(InterfaceDeclaration declaration);

        T VisitEnumDeclaration(EnumDeclaration declaration);

        T VisitTypeAliasDeclaration(TypeAliasDeclaration declaration);
    }
}
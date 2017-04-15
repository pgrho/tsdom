namespace Shipwreck.TypeScriptModels.Declarations
{
    /// <summary>
    /// Represents a visitor for namespace members that returns a value of <see cref="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the return value.</typeparam>
    public interface INamespaceMemberVisitor<T>
    {
        T VisitClassDeclaration(ClassDeclaration declaration);
        T VisitInterfaceDeclaration(InterfaceDeclaration declaration);
        T VisitEnumDeclaration(EnumDeclaration declaration);
    }
}
namespace Shipwreck.TypeScriptModels.Declarations
{
    /// <summary>
    /// Represents a visitor for module members that returns a value of <see cref="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the return value.</typeparam>
    public interface IModuleMemberVisitor<T>
    {
        T VisitClassDeclaration(ClassDeclaration declaration);
        T VisitInterfaceDeclaration(InterfaceDeclaration declaration);
        T VisitEnumDeclaration(EnumDeclaration declaration);
    }
}
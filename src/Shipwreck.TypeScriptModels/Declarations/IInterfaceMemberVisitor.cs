namespace Shipwreck.TypeScriptModels.Declarations
{
    public interface IInterfaceMemberVisitor<T>
    {
        T VisitField(FieldDeclaration member);

        T VisitIndex(IndexSignature member);

        T VisitMethod(MethodDeclaration member);

        T VisitGetAccessor(GetAccessorDeclaration member);
        T VisitSetAccessor(SetAccessorDeclaration member);
    }
}

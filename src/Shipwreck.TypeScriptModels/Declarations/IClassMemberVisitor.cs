namespace Shipwreck.TypeScriptModels.Declarations
{
    public interface IClassMemberVisitor<T>
    {
        // 8.3
        T VisitConstructor(ConstructorDeclaration member);
        T VisitField(FieldDeclaration member);
        T VisitMethod(MethodDeclaration member);

        T VisitGetAccessor(GetAccessorDeclaration member);
        T VisitSetAccessor(SetAccessorDeclaration member);

        // 8.5
        T VisitIndex(IndexSignature member);

        // TODO: 8.6 Decorators
    }
}
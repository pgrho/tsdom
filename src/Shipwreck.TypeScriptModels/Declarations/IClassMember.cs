namespace Shipwreck.TypeScriptModels.Declarations
{
    public interface IClassMember
    {
        void Accept<T>(IClassMemberVisitor<T> visitor);
    }
}
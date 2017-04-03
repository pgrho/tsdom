namespace Shipwreck.TypeScriptModels.Declarations
{
    public interface IInterfaceMember
    {
        void Accept<T>(IInterfaceMemberVisitor<T> visitor);
    }
}

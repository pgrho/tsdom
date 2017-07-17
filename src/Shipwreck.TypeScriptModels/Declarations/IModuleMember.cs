namespace Shipwreck.TypeScriptModels.Declarations
{
    public interface IModuleMember : IHasParent
    {
        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <param name="visitor">The visitor to visit this node with.</param>
        void Accept<T>(IModuleMemberVisitor<T> visitor);
    }
}
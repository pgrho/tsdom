namespace Shipwreck.TypeScriptModels.Statements
{

    public sealed class VariableBinding
    {
        public Expression Variable { get; set; }
        public Expression Initializer { get; set; }
    }
}
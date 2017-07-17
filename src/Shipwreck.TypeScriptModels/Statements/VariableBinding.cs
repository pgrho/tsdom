using Shipwreck.TypeScriptModels.Expressions;
using System.ComponentModel;

namespace Shipwreck.TypeScriptModels.Statements
{
    public sealed class VariableBinding
    {
        public VariableBinding()
        {
        }

        public VariableBinding(string variable, ITypeReference type = null, Expression initializer = null)
        {
            Variable = new IdentifierExpression(variable);
            Type = type;
            Initializer = initializer;
        }

        public VariableBinding(Expression variable, ITypeReference type = null, Expression initializer = null)
        {
            Variable = variable;
            Type = type;
            Initializer = initializer;
        }

        [DefaultValue(null)]
        public Expression Variable { get; set; }

        [DefaultValue(null)]
        public ITypeReference Type { get; set; }

        [DefaultValue(null)]
        public Expression Initializer { get; set; }

        public VariableDeclaration ToStatement()
            => new VariableDeclaration(this);
    }
}
using System;
using System.IO;

namespace Shipwreck.TypeScriptModels
{
    /// <summary>
    /// Provides the base class from which the classes that represent expression tree nodes are derived.
    /// </summary>
    public abstract class Expression : Syntax
    {
        [ThreadStatic]
        private static WeakReference<TextWriter> _CachedTextWriter;

        [ThreadStatic]
        private static WeakReference<TypeScriptWriter> _CachedTypeScriptWriter;

        public virtual ExpressionPrecedence Precedence
            => ExpressionPrecedence.Unknown;

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <param name="visitor">The visitor to visit this node with.</param>
        public abstract void Accept<T>(IExpressionVisitor<T> visitor);

        public void WriteExpression(TextWriter writer)
        {
            TextWriter w = null;
            TypeScriptWriter visitor;
            _CachedTextWriter?.TryGetTarget(out w);

            if (w == null && _CachedTypeScriptWriter.TryGetTarget(out visitor))
            {
                Accept(visitor);
            }
            else
            {
                visitor = new TypeScriptWriter(writer);
                _CachedTextWriter = new WeakReference<TextWriter>(writer);
                _CachedTypeScriptWriter = new WeakReference<TypeScriptWriter>(visitor);

                Accept(visitor);
            }
        }
    }
}
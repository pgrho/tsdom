﻿using System.Text.RegularExpressions;

namespace Shipwreck.TypeScriptModels.Expressions
{
    // 4.13
    public sealed class PropertyExpression : Expression
    {
        public PropertyExpression()
        {
        }

        public PropertyExpression(Expression obj, string property)
        {
            Object = obj;
            Property = property;
        }

        public override ExpressionPrecedence Precedence
            => ExpressionPrecedence.MemberAccess;

        public Expression Object { get; set; }

        public string Property { get; set; }

        public bool IsValidIdentifier
            => Property != null
                && Regex.IsMatch(Property, @"^(\p{Lu}|\p{Ll}|\p{Lt}|\p{Lm}|\p{Lo}|\p{Nl}|[_$]|\\u[0-9a-fA-F]{4})"
                    + @"(\p{Lu}|\p{Ll}|\p{Lt}|\p{Lm}|\p{Lo}|\p{Nl}|[_$]|\\u[0-9a-fA-F]{4}|\p{Mn}|\p{Mc}|\p{Nd}|\p{Pc})*$");

        public override void Accept<T>(IExpressionVisitor<T> visitor)
            => visitor.VisitProperty(this);
    }
}
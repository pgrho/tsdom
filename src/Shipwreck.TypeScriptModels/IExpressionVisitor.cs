using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipwreck.TypeScriptModels.Expressions;

namespace Shipwreck.TypeScriptModels
{
    /// <summary>
    /// Represents a visitor for expression trees that returns a value of <see cref="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the return value.</typeparam>
    public interface IExpressionVisitor<T>
    {
        // 4.2
        T VisitThis();

        // 4.3
        T VisitIdentifier(IdentifierExpression expression);

        // 4.4
        T VisitNull();
        T VisitBoolean(BooleanExpression expression);
        T VisitNumber(NumberExpression expression);
        T VisitString(StringExpression expression);
        T VisitRegExp(RegExpExpression expression);

        // 4.5
        T VisitObject(ObjectExpression expression);

        // 4.6
        T VisitArray(ArrayExpression expression);

        // TODO: 4.7 Template Literals

        // 4.8
        T VisitParentheses(ParenthesesExpression expression);

        // 4.9
        T VisitSuper();

        // 4.10
        T VisitFunction(FunctionExpression expression);

        // 4.11
        T VisitArrowFunction(ArrowFunctionExpression expression);

        // TODO: 4.12 Class Expressions

        // 4.13
        T VisitProperty(PropertyExpression property);

        // 4.14
        T VisitNew(NewExpression expression);

        // 4.15
        T VisitCall(CallExpression expression);

        // 4.16
        T VisitTypeAssertion(TypeAssertionExpression expression);

        // TODO: 4.17 JSX Expressions

        // 4.18
        T VisitUnary(UnaryExpression expression);

        // 4.19
        T VisitBinary(BinaryExpression expression);

        // 4.20
        T VisitConditional(ConditionalExpression expression);

        // 4.21
        T VisitAssignment(AssignmentExpression expression);

        // 4.22
        T VisitComma(CommaExpression expression);
    }
}

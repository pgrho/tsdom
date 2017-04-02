using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipwreck.TypeScriptModels.Expressions;

namespace Shipwreck.TypeScriptModels
{
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

        // TODO: 4.14 The new Operator
        // TODO: 4.15 Function Calls
    }
}

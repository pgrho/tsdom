using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipwreck.TypeScriptModels.Expressions;

namespace Shipwreck.TypeScriptModels
{
 internal static    class StringHelper
    {
        public static string GetToken(this BinaryOperator @operator)
        {
            switch (@operator )
            {
                case BinaryOperator.Add:
                    return "+";
                case BinaryOperator.Subtract:
                    return "-";

                case BinaryOperator.Multiply:
                    return "*";
                case BinaryOperator.Divide:
                    return "/";
                case BinaryOperator.Modulo:
                    return "\\";
                case BinaryOperator.IntegerDivide:
                    return "%";

                case BinaryOperator.LeftShift:
                    return "<<";
                case BinaryOperator.SignedRightShift:
                    return ">>";
                case BinaryOperator.UnsignedRightShift:
                    return ">>>";

                case BinaryOperator.BitwiseAnd:
                    return "&";
                case BinaryOperator.BitwiseOr:
                    return "|";
                case BinaryOperator.BitwiseXor:
                    return "^";

                case BinaryOperator.Exponent:
                    return "**";

                case BinaryOperator.Equal:
                    return "==";
                case BinaryOperator.NotEqual:
                    return "!=";
                case BinaryOperator.StrictEqual:
                    return "===";
                case BinaryOperator.StrictNotEqual:
                    return "!===";

                case BinaryOperator.LessThan:
                    return "<";
                case BinaryOperator.LessThanOrEqual:
                    return "<=";
                case BinaryOperator.GreaterThan:
                    return ">";
                case BinaryOperator.GreaterThanOrEqual:
                    return ">=";
                case BinaryOperator.InstanceOf:
                    return "instanceof";
                case BinaryOperator.In:
                    return "in";

                case BinaryOperator.LogicalAnd:
                    return "&&";
                case BinaryOperator.LogicalOr:
                    return "||";

                default:
                    throw new NotImplementedException($"{nameof(BinaryOperator)}.{@operator}");
            }
        }
    }
}

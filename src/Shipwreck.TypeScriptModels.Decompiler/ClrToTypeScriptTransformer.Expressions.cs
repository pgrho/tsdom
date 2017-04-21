using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Ast;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.PatternMatching;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using D = Shipwreck.TypeScriptModels.Declarations;
using E = Shipwreck.TypeScriptModels.Expressions;
using S = Shipwreck.TypeScriptModels.Statements;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    partial class ClrToTypeScriptTransformer
    {

        #region キーワード

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression, string data)
        {
            yield return new E.ThisExpression();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitBaseReferenceExpression(BaseReferenceExpression baseReferenceExpression, string data)
        {
            yield return new E.SuperExpression();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitNullReferenceExpression(NullReferenceExpression nullReferenceExpression, string data)
        {
            yield return new E.NullExpression();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitIdentifierExpression(IdentifierExpression identifierExpression, string data)
        {
            yield return new E.IdentifierExpression()
            {
                Name = identifierExpression.Identifier
            };
        }

        #endregion キーワード

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression, string data)
        {
            var e = new E.UnaryExpression();
            e.Operand = GetExpression(unaryOperatorExpression.Expression, data);
            switch (unaryOperatorExpression.Operator)
            {
                case UnaryOperatorType.Plus:
                    e.Operator = E.UnaryOperator.Plus;
                    break;

                case UnaryOperatorType.Minus:
                    e.Operator = E.UnaryOperator.Minus;
                    break;

                case UnaryOperatorType.Increment:
                    e.Operator = E.UnaryOperator.PrefixIncrement;
                    break;

                case UnaryOperatorType.PostIncrement:
                    e.Operator = E.UnaryOperator.PostfixIncrement;
                    break;

                case UnaryOperatorType.Decrement:
                    e.Operator = E.UnaryOperator.PrefixDecrement;
                    break;

                case UnaryOperatorType.PostDecrement:
                    e.Operator = E.UnaryOperator.PostfixDecrement;
                    break;

                case UnaryOperatorType.Not:
                    e.Operator = E.UnaryOperator.LogicalNot;
                    break;

                case UnaryOperatorType.BitNot:
                    e.Operator = E.UnaryOperator.BitwiseNot;
                    break;

                case UnaryOperatorType.Await:
                //TODO: await

                default:
                    throw new NotImplementedException();
            }

            yield return e;
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitInvocationExpression(InvocationExpression invocationExpression, string data)
        {
            var inv = new E.CallExpression();

            var mre = invocationExpression.Target as MemberReferenceExpression;
            if (mre != null)
            {
                // TODO: method mapping

                foreach (var t in mre.TypeArguments)
                {
                    inv.TypeArguments.Add(GetTypeReference(t));
                }

                inv.Type = new E.PropertyExpression()
                {
                    Object = GetExpression(mre.Target, data),
                    Property = mre.MemberName
                };
            }
            else
            {
                inv.Type = GetExpression(invocationExpression.Target, data);
            }

            foreach (var p in invocationExpression.Arguments)
            {
                inv.Parameters.Add(GetExpression(p, data));
            }

            yield return inv;
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, string data)
        {
            var e = new E.BinaryExpression();
            e.Left = GetExpression(binaryOperatorExpression.Left, data);
            e.Right = GetExpression(binaryOperatorExpression.Right, data);
            switch (binaryOperatorExpression.Operator)
            {
                case BinaryOperatorType.Add:
                    e.Operator = E.BinaryOperator.Add;
                    break;
                case BinaryOperatorType.Subtract:
                    e.Operator = E.BinaryOperator.Subtract;
                    break;

                case BinaryOperatorType.Multiply:
                    // TODO: integer divide
                    e.Operator = E.BinaryOperator.Multiply;
                    break;
                case BinaryOperatorType.Divide:
                    e.Operator = E.BinaryOperator.Divide;
                    break;
                case BinaryOperatorType.Modulus:
                    e.Operator = E.BinaryOperator.Modulo;
                    break;

                case BinaryOperatorType.ShiftLeft:
                    e.Operator = E.BinaryOperator.LeftShift;
                    break;
                case BinaryOperatorType.ShiftRight:
                    // TODO: Unsigned
                    e.Operator = E.BinaryOperator.SignedRightShift;
                    break;

                case BinaryOperatorType.BitwiseAnd:
                    e.Operator = E.BinaryOperator.BitwiseAnd;
                    break;
                case BinaryOperatorType.BitwiseOr:
                    e.Operator = E.BinaryOperator.BitwiseOr;
                    break;
                case BinaryOperatorType.ExclusiveOr:
                    e.Operator = E.BinaryOperator.BitwiseXor;
                    break;

                case BinaryOperatorType.Equality:
                    e.Operator = E.BinaryOperator.StrictEqual;
                    break;
                case BinaryOperatorType.InEquality:
                    e.Operator = E.BinaryOperator.StrictNotEqual;
                    break;

                case BinaryOperatorType.LessThan:
                    e.Operator = E.BinaryOperator.LessThan;
                    break;
                case BinaryOperatorType.LessThanOrEqual:
                    e.Operator = E.BinaryOperator.LessThanOrEqual;
                    break;
                case BinaryOperatorType.GreaterThan:
                    e.Operator = E.BinaryOperator.GreaterThan;
                    break;
                case BinaryOperatorType.GreaterThanOrEqual:
                    e.Operator = E.BinaryOperator.GreaterThanOrEqual;
                    break;

                case BinaryOperatorType.ConditionalAnd:
                    e.Operator = E.BinaryOperator.LogicalAnd;
                    break;
                case BinaryOperatorType.ConditionalOr:
                    e.Operator = E.BinaryOperator.LogicalOr;
                    break;
                default:
                    throw new NotImplementedException();
            }
            yield return e;
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression, string data)
        {
            if (memberReferenceExpression.TypeArguments.Any())
            {
                throw new NotImplementedException();
            }

            yield return new E.PropertyExpression()
            {
                Object = GetExpression(memberReferenceExpression.Target, data),
                Property = memberReferenceExpression.MemberName
            };
        }

        private Expression GetExpression(ICSharpCode.NRefactory.CSharp.Expression expression, string data)
               => expression?.IsNull != false ? null : expression.AcceptVisitor(this, data).Cast<Expression>().Single();

    }
}
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
        private Expression Concat(Expression current, Expression other)
        {
            if (current == null)
            {
                return other;
            }
            else
            {
                return new E.CommaExpression()
                {
                    Left = current,
                    Right = other
                };
            }
        }

        private static E.BinaryOperator GetOperator(BinaryOperatorType @operator)
        {
            switch (@operator)
            {
                case BinaryOperatorType.Add:
                    return E.BinaryOperator.Add;

                case BinaryOperatorType.Subtract:
                    return E.BinaryOperator.Subtract;

                case BinaryOperatorType.Multiply:
                    // TODO: integer divide
                    return E.BinaryOperator.Multiply;

                case BinaryOperatorType.Divide:
                    return E.BinaryOperator.Divide;

                case BinaryOperatorType.Modulus:
                    return E.BinaryOperator.Modulo;

                case BinaryOperatorType.ShiftLeft:
                    return E.BinaryOperator.LeftShift;

                case BinaryOperatorType.ShiftRight:
                    // TODO: Unsigned
                    return E.BinaryOperator.SignedRightShift;

                case BinaryOperatorType.BitwiseAnd:
                    return E.BinaryOperator.BitwiseAnd;

                case BinaryOperatorType.BitwiseOr:
                    return E.BinaryOperator.BitwiseOr;

                case BinaryOperatorType.ExclusiveOr:
                    return E.BinaryOperator.BitwiseXor;

                case BinaryOperatorType.Equality:
                    return E.BinaryOperator.StrictEqual;

                case BinaryOperatorType.InEquality:
                    return E.BinaryOperator.StrictNotEqual;

                case BinaryOperatorType.LessThan:
                    return E.BinaryOperator.LessThan;

                case BinaryOperatorType.LessThanOrEqual:
                    return E.BinaryOperator.LessThanOrEqual;

                case BinaryOperatorType.GreaterThan:
                    return E.BinaryOperator.GreaterThan;

                case BinaryOperatorType.GreaterThanOrEqual:
                    return E.BinaryOperator.GreaterThanOrEqual;

                case BinaryOperatorType.ConditionalAnd:
                    return E.BinaryOperator.LogicalAnd;

                case BinaryOperatorType.ConditionalOr:
                    return E.BinaryOperator.LogicalOr;

                default:
                    throw new NotImplementedException();
            }
        }

        private static E.BinaryOperator GetOperator(AssignmentOperatorType @operator)
        {
            switch (@operator)
            {
                case AssignmentOperatorType.Add:
                    return E.BinaryOperator.Add;

                case AssignmentOperatorType.Subtract:
                    return E.BinaryOperator.Subtract;

                case AssignmentOperatorType.Multiply:
                    // TODO: integer divide
                    return E.BinaryOperator.Multiply;

                case AssignmentOperatorType.Divide:
                    return E.BinaryOperator.Divide;

                case AssignmentOperatorType.Modulus:
                    return E.BinaryOperator.Modulo;

                case AssignmentOperatorType.ShiftLeft:
                    return E.BinaryOperator.LeftShift;

                case AssignmentOperatorType.ShiftRight:
                    // TODO: Unsigned
                    return E.BinaryOperator.SignedRightShift;

                case AssignmentOperatorType.BitwiseAnd:
                    return E.BinaryOperator.BitwiseAnd;

                case AssignmentOperatorType.BitwiseOr:
                    return E.BinaryOperator.BitwiseOr;

                case AssignmentOperatorType.ExclusiveOr:
                    return E.BinaryOperator.BitwiseXor;

                default:
                    throw new NotImplementedException();
            }
        }

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

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression, string data)
            => parenthesizedExpression.Expression.AcceptVisitor(this, data);

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

                inv.Target = new E.PropertyExpression()
                {
                    Object = GetExpression(mre.Target, data),
                    Property = mre.MemberName
                };
            }
            else
            {
                inv.Target = GetExpression(invocationExpression.Target, data);
            }

            foreach (var p in invocationExpression.Arguments)
            {
                inv.Parameters.Add(GetExpression(p, data));
            }

            yield return inv;
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitIndexerExpression(IndexerExpression indexerExpression, string data)
        {
            yield return new E.IndexerExpression()
            {
                Object = GetExpression(indexerExpression.Target, data),
                Index = GetExpression(indexerExpression.Arguments.Single(), data),
            };
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression, string data)
        {
            throw new NotImplementedException();
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, string data)
        {
            yield return new E.BinaryExpression()
            {
                Left = GetExpression(binaryOperatorExpression.Left, data),
                Right = GetExpression(binaryOperatorExpression.Right, data),
                Operator = GetOperator(binaryOperatorExpression.Operator)
            };
        }

        IEnumerable<Syntax> IAstVisitor<string, IEnumerable<Syntax>>.VisitAssignmentExpression(AssignmentExpression assignmentExpression, string data)
        {
            yield return new E.AssignmentExpression()
            {
                Target = GetExpression(assignmentExpression.Left, data),
                Value = GetExpression(assignmentExpression.Right, data),
                CompoundOperator = assignmentExpression.Operator == AssignmentOperatorType.Assign ? E.BinaryOperator.Default : GetOperator(assignmentExpression.Operator)
            };
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
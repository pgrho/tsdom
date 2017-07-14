using ICSharpCode.NRefactory.CSharp;
using System.Collections.Generic;
using System.Linq;
using E = Shipwreck.TypeScriptModels.Expressions;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    partial class ILTranslator
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
                    throw GetNotImplementedException();
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
                    throw GetNotImplementedException();
            }
        }

        #region キーワード

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression, ILTransformationContext data)
            => OnVisiting(data, thisReferenceExpression, VisitingThisReferenceExpression)
                ?? OnVisited(data, thisReferenceExpression, VisitedThisReferenceExpression, TranslateThisReferenceExpresssion(thisReferenceExpression, data));

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitBaseReferenceExpression(BaseReferenceExpression baseReferenceExpression, ILTransformationContext data)
            => OnVisiting(data, baseReferenceExpression, VisitingBaseReferenceExpression)
                ?? OnVisited(data, baseReferenceExpression, VisitedBaseReferenceExpression, TranslateBaseReferenceExpresssion(baseReferenceExpression, data));

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitNullReferenceExpression(NullReferenceExpression nullReferenceExpression, ILTransformationContext data)
            => OnVisiting(data, nullReferenceExpression, VisitingNullReferenceExpression)
                ?? OnVisited(data, nullReferenceExpression, VisitedNullReferenceExpression, TranslateNullReferenceExpresssion(nullReferenceExpression, data));

        protected virtual Expression TranslateThisReferenceExpresssion(ThisReferenceExpression thisReferenceExpression, ILTransformationContext data)
            => new E.ThisExpression();

        protected virtual Expression TranslateBaseReferenceExpresssion(BaseReferenceExpression baseReferenceExpression, ILTransformationContext data)
            => new E.SuperExpression();

        protected virtual Expression TranslateNullReferenceExpresssion(NullReferenceExpression nullReferenceExpression, ILTransformationContext data)
            => new E.NullExpression();

        #endregion キーワード

        #region 識別子

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitIdentifierExpression(IdentifierExpression identifierExpression, ILTransformationContext data)
            => OnVisiting(data, identifierExpression, VisitingIdentifierExpression)
            ?? OnVisited(data, identifierExpression, VisitedIdentifierExpression, TranslateIdentifierExpression(identifierExpression, data));

        protected virtual IEnumerable<Syntax> TranslateIdentifierExpression(IdentifierExpression identifierExpression, ILTransformationContext data)
        {
            yield return new E.IdentifierExpression()
            {
                Name = identifierExpression.Identifier
            };
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitTypeReferenceExpression(TypeReferenceExpression typeReferenceExpression, ILTransformationContext data)
            => OnVisiting(data, typeReferenceExpression, VisitingTypeReferenceExpression)
            ?? OnVisited(data, typeReferenceExpression, VisitedTypeReferenceExpression, TranslateTypeReferenceExpression(typeReferenceExpression, data));

        protected virtual IEnumerable<Syntax> TranslateTypeReferenceExpression(TypeReferenceExpression typeReferenceExpression, ILTransformationContext data)
        {
            yield return new E.IdentifierExpression()
            {
                Name = GetTypeName(typeReferenceExpression.Type)
            };
        }

        #endregion 識別子

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression, ILTransformationContext data)
            => OnVisiting(data, parenthesizedExpression, VisitingParenthesizedExpression)
            ?? OnVisited(data, parenthesizedExpression, VisitedParenthesizedExpression, TranslateParenthesizedExpression(parenthesizedExpression, data));

        protected virtual IEnumerable<Syntax> TranslateParenthesizedExpression(ParenthesizedExpression parenthesizedExpression, ILTransformationContext data)
                    => parenthesizedExpression.Expression.AcceptVisitor(this, data);

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression, ILTransformationContext data)
            => OnVisiting(data, unaryOperatorExpression, VisitingUnaryOperatorExpression)
            ?? OnVisited(data, unaryOperatorExpression, VisitedUnaryOperatorExpression, TranslateUnaryOperatorExpression(unaryOperatorExpression, data));

        protected virtual IEnumerable<Syntax> TranslateUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression, ILTransformationContext data)
        {
            if (unaryOperatorExpression.Operator == UnaryOperatorType.Await)
            {
                data.HasAwait = true;

                return new[]
                {
                    new E.AwaitExpression()
                    {
                        Operand = GetExpression(unaryOperatorExpression.Expression, data)
                    }
                };
            }
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

                default:
                    throw GetNotImplementedException();
            }

            return new[] { e };
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitInvocationExpression(InvocationExpression invocationExpression, ILTransformationContext data)
            => OnVisiting(data, invocationExpression, VisitingInvocationExpression)
            ?? OnVisited(data, invocationExpression, VisitedInvocationExpression, TranslateInvocationExpression(invocationExpression, data));

        protected virtual E.CallExpression TranslateInvocationExpression(InvocationExpression invocationExpression, ILTransformationContext data)
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

            return inv;
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitIndexerExpression(IndexerExpression indexerExpression, ILTransformationContext data)
            => OnVisiting(data, indexerExpression, VisitingIndexerExpression)
            ?? OnVisited(data, indexerExpression, VisitedIndexerExpression, TranslateIndexerExpression(indexerExpression, data));

        protected virtual IEnumerable<Syntax> TranslateIndexerExpression(IndexerExpression indexerExpression, ILTransformationContext data)
        {
            yield return new E.IndexerExpression()
            {
                Object = GetExpression(indexerExpression.Target, data),
                Index = GetExpression(indexerExpression.Arguments.Single(), data),
            };
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression, ILTransformationContext data)
            => OnVisiting(data, objectCreateExpression, VisitingObjectCreateExpression)
            ?? OnVisited(data, objectCreateExpression, VisitedObjectCreateExpression, TranslateObjectCreateExpression(objectCreateExpression, data));

        protected virtual IEnumerable<Syntax> TranslateObjectCreateExpression(ObjectCreateExpression objectCreateExpression, ILTransformationContext data)
        {
            var ne = new E.NewExpression();
            ne.Type = GetTypeReference(objectCreateExpression.Type).ToExpression();

            foreach (var p in objectCreateExpression.Arguments)
            {
                ne.Parameters.Add(p.AcceptVisitor(this, data).Cast<Expression>().Single());
            }

            // TODO: initializer

            yield return ne;
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, ILTransformationContext data)
            => OnVisiting(data, binaryOperatorExpression, VisitingBinaryOperatorExpression)
            ?? OnVisited(data, binaryOperatorExpression, VisitedBinaryOperatorExpression, TranslateBinaryOperatorExpression(binaryOperatorExpression, data));

        protected virtual IEnumerable<Syntax> TranslateBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, ILTransformationContext data)
        {
            yield return new E.BinaryExpression()
            {
                Left = GetExpression(binaryOperatorExpression.Left, data),
                Right = GetExpression(binaryOperatorExpression.Right, data),
                Operator = GetOperator(binaryOperatorExpression.Operator)
            };
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitAssignmentExpression(AssignmentExpression assignmentExpression, ILTransformationContext data)
            => OnVisiting(data, assignmentExpression, VisitingAssignmentExpression)
            ?? OnVisited(data, assignmentExpression, VisitedAssignmentExpression, TranslateAssignmentExpression(assignmentExpression, data));

        protected virtual IEnumerable<Syntax> TranslateAssignmentExpression(AssignmentExpression assignmentExpression, ILTransformationContext data)
        {
            yield return new E.AssignmentExpression()
            {
                Target = GetExpression(assignmentExpression.Left, data),
                Value = GetExpression(assignmentExpression.Right, data),
                CompoundOperator = assignmentExpression.Operator == AssignmentOperatorType.Assign ? E.BinaryOperator.Default : GetOperator(assignmentExpression.Operator)
            };
        }

        IEnumerable<Syntax> IAstVisitor<ILTransformationContext, IEnumerable<Syntax>>.VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression, ILTransformationContext data)
            => OnVisiting(data, memberReferenceExpression, VisitingMemberReferenceExpression)
            ?? OnVisited(data, memberReferenceExpression, VisitedMemberReferenceExpression, TranslateMemberReferenceExpression(memberReferenceExpression, data));

        protected virtual IEnumerable<Syntax> TranslateMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression, ILTransformationContext data)
        {
            if (memberReferenceExpression.TypeArguments.Any())
            {
                throw GetNotImplementedException();
            }

            yield return new E.PropertyExpression()
            {
                Object = GetExpression(memberReferenceExpression.Target, data),
                Property = memberReferenceExpression.MemberName
            };
        }

        private Expression GetExpression(ICSharpCode.NRefactory.CSharp.Expression expression, ILTransformationContext data)
               => expression?.IsNull != false ? null : expression.AcceptVisitor(this, data).Cast<Expression>().Single();
    }
}
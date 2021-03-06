﻿using ICSharpCode.Decompiler.Ast;
using ICSharpCode.NRefactory.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression, ILTranslationContext data)
            => OnVisiting(data, thisReferenceExpression, VisitingThisReferenceExpression)
                ?? OnVisited(data, thisReferenceExpression, VisitedThisReferenceExpression, TranslateThisReferenceExpresssion(thisReferenceExpression, data));

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitBaseReferenceExpression(BaseReferenceExpression baseReferenceExpression, ILTranslationContext data)
            => OnVisiting(data, baseReferenceExpression, VisitingBaseReferenceExpression)
                ?? OnVisited(data, baseReferenceExpression, VisitedBaseReferenceExpression, TranslateBaseReferenceExpresssion(baseReferenceExpression, data));

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitNullReferenceExpression(NullReferenceExpression nullReferenceExpression, ILTranslationContext data)
            => OnVisiting(data, nullReferenceExpression, VisitingNullReferenceExpression)
                ?? OnVisited(data, nullReferenceExpression, VisitedNullReferenceExpression, TranslateNullReferenceExpresssion(nullReferenceExpression, data));

        protected virtual Expression TranslateThisReferenceExpresssion(ThisReferenceExpression thisReferenceExpression, ILTranslationContext data)
            => new E.ThisExpression();

        protected virtual Expression TranslateBaseReferenceExpresssion(BaseReferenceExpression baseReferenceExpression, ILTranslationContext data)
            => new E.SuperExpression();

        protected virtual Expression TranslateNullReferenceExpresssion(NullReferenceExpression nullReferenceExpression, ILTranslationContext data)
            => new E.NullExpression();

        #endregion キーワード

        #region 識別子

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitIdentifierExpression(IdentifierExpression identifierExpression, ILTranslationContext data)
            => OnVisiting(data, identifierExpression, VisitingIdentifierExpression)
            ?? OnVisited(data, identifierExpression, VisitedIdentifierExpression, TranslateIdentifierExpression(identifierExpression, data));

        protected virtual IEnumerable<Syntax> TranslateIdentifierExpression(IdentifierExpression identifierExpression, ILTranslationContext data)
        {
            yield return new E.IdentifierExpression()
            {
                Name = identifierExpression.Identifier
            };
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitTypeReferenceExpression(TypeReferenceExpression typeReferenceExpression, ILTranslationContext data)
            => OnVisiting(data, typeReferenceExpression, VisitingTypeReferenceExpression)
            ?? OnVisited(data, typeReferenceExpression, VisitedTypeReferenceExpression, TranslateTypeReferenceExpression(typeReferenceExpression, data));

        protected virtual IEnumerable<Syntax> TranslateTypeReferenceExpression(TypeReferenceExpression typeReferenceExpression, ILTranslationContext data)
        {
            yield return new E.IdentifierExpression()
            {
                Name = GetTypeName(typeReferenceExpression.Type)
            };
        }

        #endregion 識別子

        #region リテラル

        private static PropertyInfo _PrimitiveExpressionValueProperty;

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitPrimitiveExpression(PrimitiveExpression primitiveExpression, ILTranslationContext data)
            => OnVisiting(data, primitiveExpression, VisitingPrimitiveExpression)
            ?? OnVisited(data, primitiveExpression, VisitedPrimitiveExpression, TranslatePrimitiveExpression(primitiveExpression, data));

        protected virtual IEnumerable<Syntax> TranslatePrimitiveExpression(PrimitiveExpression primitiveExpression, ILTranslationContext data)
        {
            var ex = ExpressionBuilder.Literal(primitiveExpression.Value);

            if (primitiveExpression.Value != null)
            {
                ex.SetAnnotation(
                    _PrimitiveExpressionValueProperty ?? (_PrimitiveExpressionValueProperty = typeof(PrimitiveExpression).GetProperty("Value")),
                    primitiveExpression.Value);
                ex.SetAnnotation(typeof(Type), primitiveExpression.Value.GetType());
            }

            yield return ex;
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitDefaultValueExpression(DefaultValueExpression defaultValueExpression, ILTranslationContext data)
            => OnVisiting(data, defaultValueExpression, VisitingDefaultValueExpression)
            ?? OnVisited(data, defaultValueExpression, VisitedDefaultValueExpression, TranslateDefaultValueExpression(defaultValueExpression, data));

        protected virtual IEnumerable<Syntax> TranslateDefaultValueExpression(DefaultValueExpression defaultValueExpression, ILTranslationContext data)
        {
            var ti = defaultValueExpression.Annotation<TypeInformation>()?.InferredType;

            if (ti?.Namespace == nameof(System))
            {
                switch (ti.Name)
                {
                    case nameof(Boolean):
                        yield return new E.BooleanExpression(false);
                        yield break;

                    case nameof(Byte):
                    case nameof(SByte):
                    case nameof(Int16):
                    case nameof(UInt16):
                    case nameof(Int32):
                    case nameof(UInt32):
                    case nameof(Int64):
                    case nameof(UInt64):
                    case nameof(Single):
                    case nameof(Double):
                    case nameof(Decimal):
                        yield return new E.NumberExpression(0);
                        yield break;
                }
            }

            if (ti?.IsValueType == true)
            {
                var ct = ti.ResolveClrType();

                if (ct?.IsEnum == true)
                {
                    var n = Enum.GetName(ct, ((IConvertible)0).ToType(Enum.GetUnderlyingType(ct), null));
                    if (n == null)
                    {
                        yield return new E.NumberExpression(0);
                    }
                    else
                    {
                        yield return new E.IdentifierExpression(ct.FullName).Property(n);
                    }
                    yield break;
                }
            }

            yield return new E.NullExpression();
            yield break;
        }

        #endregion リテラル

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression, ILTranslationContext data)
            => OnVisiting(data, parenthesizedExpression, VisitingParenthesizedExpression)
            ?? OnVisited(data, parenthesizedExpression, VisitedParenthesizedExpression, TranslateParenthesizedExpression(parenthesizedExpression, data));

        protected virtual IEnumerable<Syntax> TranslateParenthesizedExpression(ParenthesizedExpression parenthesizedExpression, ILTranslationContext data)
                    => parenthesizedExpression.Expression.AcceptVisitor(this, data);

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression, ILTranslationContext data)
            => OnVisiting(data, unaryOperatorExpression, VisitingUnaryOperatorExpression)
            ?? OnVisited(data, unaryOperatorExpression, VisitedUnaryOperatorExpression, TranslateUnaryOperatorExpression(unaryOperatorExpression, data));

        protected virtual IEnumerable<Syntax> TranslateUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression, ILTranslationContext data)
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

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitInvocationExpression(InvocationExpression invocationExpression, ILTranslationContext data)
            => OnVisiting(data, invocationExpression, VisitingInvocationExpression)
            ?? OnVisited(data, invocationExpression, VisitedInvocationExpression, TranslateInvocationExpression(invocationExpression, data));

        protected virtual E.CallExpression TranslateInvocationExpression(InvocationExpression invocationExpression, ILTranslationContext data)
        {
            var inv = new E.CallExpression();

            var mre = invocationExpression.Target as MemberReferenceExpression;
            if (mre != null)
            {
                // TODO: method mapping

                foreach (var t in mre.TypeArguments)
                {
                    inv.TypeArguments.Add(ResolveType(mre, t));
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

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitIndexerExpression(IndexerExpression indexerExpression, ILTranslationContext data)
            => OnVisiting(data, indexerExpression, VisitingIndexerExpression)
            ?? OnVisited(data, indexerExpression, VisitedIndexerExpression, TranslateIndexerExpression(indexerExpression, data));

        protected virtual IEnumerable<Syntax> TranslateIndexerExpression(IndexerExpression indexerExpression, ILTranslationContext data)
        {
            yield return new E.IndexerExpression()
            {
                Object = GetExpression(indexerExpression.Target, data),
                Index = GetExpression(indexerExpression.Arguments.Single(), data),
            };
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression, ILTranslationContext data)
            => OnVisiting(data, objectCreateExpression, VisitingObjectCreateExpression)
            ?? OnVisited(data, objectCreateExpression, VisitedObjectCreateExpression, TranslateObjectCreateExpression(objectCreateExpression, data));

        protected virtual IEnumerable<Syntax> TranslateObjectCreateExpression(ObjectCreateExpression objectCreateExpression, ILTranslationContext data)
        {
            E.NewExpression ne;

            var clrType = objectCreateExpression.Type.ResolveClrType();
            if (clrType != null)
            {
                if (typeof(MulticastDelegate).IsAssignableFrom(clrType))
                {
                    var p = objectCreateExpression.Arguments.OfType<MemberReferenceExpression>().FirstOrDefault();

                    if (p != null)
                    {
                        var t = p.Target.AcceptVisitor(this, data).ToArray();
                        if (t.Length == 1)
                        {
                            var te = (Expression)t[0];
                            yield return te.Property(p.MemberName).Property("bind").Call(te);

                            yield break;
                        }
                    }
                }

                ne = new E.NewExpression();
                ne.Type = ResolveClrType(objectCreateExpression, clrType).ToExpression();
            }
            else
            {
                ne = new E.NewExpression();
                ne.Type = ResolveType(objectCreateExpression, objectCreateExpression.Type).ToExpression();
            }

            foreach (var p in objectCreateExpression.Arguments)
            {
                ne.Parameters.Add(p.AcceptVisitor(this, data).Cast<Expression>().Single());
            }

            // TODO: initializer

            yield return ne;
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, ILTranslationContext data)
            => OnVisiting(data, binaryOperatorExpression, VisitingBinaryOperatorExpression)
            ?? OnVisited(data, binaryOperatorExpression, VisitedBinaryOperatorExpression, TranslateBinaryOperatorExpression(binaryOperatorExpression, data));

        protected virtual IEnumerable<Syntax> TranslateBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, ILTranslationContext data)
        {
            yield return new E.BinaryExpression()
            {
                Left = GetExpression(binaryOperatorExpression.Left, data),
                Right = GetExpression(binaryOperatorExpression.Right, data),
                Operator = GetOperator(binaryOperatorExpression.Operator)
            };
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitAssignmentExpression(AssignmentExpression assignmentExpression, ILTranslationContext data)
            => OnVisiting(data, assignmentExpression, VisitingAssignmentExpression)
            ?? OnVisited(data, assignmentExpression, VisitedAssignmentExpression, TranslateAssignmentExpression(assignmentExpression, data));

        protected virtual IEnumerable<Syntax> TranslateAssignmentExpression(AssignmentExpression assignmentExpression, ILTranslationContext data)
        {
            yield return new E.AssignmentExpression()
            {
                Target = GetExpression(assignmentExpression.Left, data),
                Value = GetExpression(assignmentExpression.Right, data),
                CompoundOperator = assignmentExpression.Operator == AssignmentOperatorType.Assign ? E.BinaryOperator.Default : GetOperator(assignmentExpression.Operator)
            };
        }

        IEnumerable<Syntax> IAstVisitor<ILTranslationContext, IEnumerable<Syntax>>.VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression, ILTranslationContext data)
            => OnVisiting(data, memberReferenceExpression, VisitingMemberReferenceExpression)
            ?? OnVisited(data, memberReferenceExpression, VisitedMemberReferenceExpression, TranslateMemberReferenceExpression(memberReferenceExpression, data));

        protected virtual IEnumerable<Syntax> TranslateMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression, ILTranslationContext data)
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

        private Expression GetExpression(ICSharpCode.NRefactory.CSharp.Expression expression, ILTranslationContext data)
               => expression?.IsNull != false ? null : expression.AcceptVisitor(this, data).Cast<Expression>().Single();
    }
}
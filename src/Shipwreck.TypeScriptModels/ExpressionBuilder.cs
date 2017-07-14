﻿using Shipwreck.TypeScriptModels.Expressions;

namespace Shipwreck.TypeScriptModels
{
    public static class ExpressionBuilder
    {
        #region BinaryExpression

        public static BinaryExpression MakeBinary(this Expression left, Expression right, BinaryOperator @operator)
            => new BinaryExpression()
            {
                Left = left,
                Right = right,
                Operator = @operator
            };

        public static BinaryExpression AddedBy(this Expression left, Expression right)
            => left.MakeBinary(right, BinaryOperator.Add);

        public static BinaryExpression SubtractedBy(this Expression left, Expression right)
            => left.MakeBinary(right, BinaryOperator.Subtract);

        public static BinaryExpression IsEqualTo(this Expression left, Expression right)
            => left.MakeBinary(right, BinaryOperator.Equal);

        public static BinaryExpression IsStrictEqualTo(this Expression left, Expression right)
            => left.MakeBinary(right, BinaryOperator.StrictEqual);

        public static BinaryExpression IsNotEqualTo(this Expression left, Expression right)
            => left.MakeBinary(right, BinaryOperator.NotEqual);

        public static BinaryExpression IsStrictNotEqualTo(this Expression left, Expression right)
            => left.MakeBinary(right, BinaryOperator.StrictNotEqual);

        public static BinaryExpression LogicalOrWith(this Expression left, Expression right)
            => left.MakeBinary(right, BinaryOperator.LogicalOr);

        #endregion BinaryExpression

        #region AssignmentExpression

        public static AssignmentExpression MakeAssignment(this Expression target, Expression value, BinaryOperator compoundOperator)
            => new AssignmentExpression()
            {
                Target = target,
                Value = value,
                CompoundOperator = compoundOperator
            };

        public static AssignmentExpression AssignedBy(this Expression target, Expression value)
            => target.MakeAssignment(value, BinaryOperator.Default);

        #endregion AssignmentExpression

        public static PropertyExpression Property(this Expression obj, string property)
            => new PropertyExpression(obj, property);

        public static CallExpression Call(this Expression target)
            => new CallExpression() { Target = target };

        public static CallExpression Call(this Expression target, Expression arg)
        {
            var r = new CallExpression() { Target = target };
            r.Parameters.Add(arg);

            return r;
        }

        public static CallExpression Call(this Expression target, params Expression[] args)
        {
            var r = new CallExpression() { Target = target };
            foreach (var a in args)
            {
                r.Parameters.Add(a);
            }

            return r;
        }

        public static Statements.ExpressionStatement ToStatement(this Expression expression)
            => new Statements.ExpressionStatement()
            {
                Expression = expression
            };

        public static Statements.ReturnStatement ToReturn(this Expression value)
            => new Statements.ReturnStatement()
            {
                Value = value
            };
    }
}
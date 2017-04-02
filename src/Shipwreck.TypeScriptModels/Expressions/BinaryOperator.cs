namespace Shipwreck.TypeScriptModels.Expressions
{
    public enum BinaryOperator
    {
        Default,

        Add,
        Subtract,

        Multiply,
        Divide,
        Modulo,
        IntegerDivide,

        LeftShift,
        SignedRightShift,
        UnsignedRightShift,

        BitwiseAnd,
        BitwiseOr,
        BitwiseXor,

        Exponent,

        Equal,
        NotEqual,
        StrictEqual,
        StrictNotEqual,

        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
        InstanceOf,
        In,

        LogicalAnd,
        LogicalOr,
    }
}
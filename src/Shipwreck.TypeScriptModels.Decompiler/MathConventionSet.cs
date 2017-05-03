using System;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    public sealed class MathConventionSet : ILTranslationConventionSet
    {
        // https://msdn.microsoft.com/ja-jp/library/system.math(v=vs.110).aspx https://developer.mozilla.org/ja/docs/Web/JavaScript/Reference/Global_Objects/Math

        private static readonly ILTranslationConvention[] _Conventions = {
            new MethodNameConvention(typeof(Math), nameof(Math.Abs), "abs", nameof(Math)),
            new MethodNameConvention(typeof(Math), nameof(Math.Acos), "acos", nameof(Math)),
            new MethodNameConvention(typeof(Math), nameof(Math.Asin), "asin", nameof(Math)),
            new MethodNameConvention(typeof(Math), nameof(Math.Atan), "atan", nameof(Math)),
            new MethodNameConvention(typeof(Math), nameof(Math.Atan2), "atan2", nameof(Math)),
            // TODO: BigMul
            new MethodNameConvention(typeof(Math), nameof(Math.Ceiling), "ceil", nameof(Math)),
            new MethodNameConvention(typeof(Math), nameof(Math.Cos), "cos", nameof(Math)),
            new MethodNameConvention(typeof(Math), nameof(Math.Cosh), "cosh", nameof(Math)),
            // TODO: DivRem
            new MethodNameConvention(typeof(Math), nameof(Math.Exp), "exp", nameof(Math)),
            new MethodNameConvention(typeof(Math), nameof(Math.Floor), "floor", nameof(Math)),
            // TODO: IEEERemainder
            new MethodNameConvention(typeof(Math), nameof(Math.Log), "log", nameof(Math)),
            new MethodNameConvention(typeof(Math), nameof(Math.Log10), "log10", nameof(Math)),

            new MethodNameConvention(typeof(Math), nameof(Math.Max), "max", nameof(Math)),
            new MethodNameConvention(typeof(Math), nameof(Math.Min), "min", nameof(Math)),
            // TODO: Flatten nested invocations

            new MethodNameConvention(typeof(Math), nameof(Math.Pow), "pow", nameof(Math)),
            new MethodNameConvention(typeof(Math), nameof(Math.Round), "round", nameof(Math)),

            new MethodNameConvention(typeof(Math), nameof(Math.Sign), "sign", nameof(Math)),
            new MethodNameConvention(typeof(Math), nameof(Math.Sin), "sin", nameof(Math)),
            new MethodNameConvention(typeof(Math), nameof(Math.Sinh), "sinh", nameof(Math)),
            new MethodNameConvention(typeof(Math), nameof(Math.Sqrt), "sqrt", nameof(Math)),
            new MethodNameConvention(typeof(Math), nameof(Math.Tan), "tan", nameof(Math)),
            new MethodNameConvention(typeof(Math), nameof(Math.Tanh), "tanh", nameof(Math)),
            new MethodNameConvention(typeof(Math), nameof(Math.Truncate), "trunc", nameof(Math)),

            // TODO: Math.random
        };

        /// <summary>
        /// <see cref="MathConventionSet" /> クラスの新しいインスタンスを初期化します。
        /// </summary>
        public MathConventionSet()
            : base(_Conventions, false)
        {
        }
    }
}
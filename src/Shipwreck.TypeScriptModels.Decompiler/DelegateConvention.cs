using ICSharpCode.NRefactory.CSharp;
using System;
using System.Collections.ObjectModel;
using D = Shipwreck.TypeScriptModels.Declarations;
using E = Shipwreck.TypeScriptModels.Expressions;
using S = Shipwreck.TypeScriptModels.Statements;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    public class DelegateConvention : ILTranslationConvention
    {
        public override void ApplyTo(ILTranslator translator)
        {
            translator.ResolvingClrType -= Translator_ResolvingClrType;
            translator.ResolvingClrType += Translator_ResolvingClrType;

            translator.VisitedAssignmentExpression -= Translator_VisitedAssignmentExpression;
            translator.VisitedAssignmentExpression += Translator_VisitedAssignmentExpression;

            translator.VisitedInvocationExpression -= Translator_VisitedInvocationExpression;
            translator.VisitedInvocationExpression += Translator_VisitedInvocationExpression;
        }

        private void Translator_ResolvingClrType(object sender, ResolvingTypeEventArgs<Type> e)
        {
            if (typeof(MulticastDelegate).IsAssignableFrom(e.OriginalType))
            {
                var ilt = (ILTranslator)sender;
                var m = e.OriginalType.GetMethod(nameof(Action.Invoke));

                var ft = new D.FunctionType();
                foreach (var p in m.GetParameters())
                {
                    bool b;
                    ft.Parameters.Add(new D.Parameter()
                    {
                        ParameterName = p.Name,
                        ParameterType = ilt.ResolveClrType(p.ParameterType, out b),
                        IsOptional = b
                    });
                }
                ft.ReturnType = ilt.ResolveClrType(m.ReturnType) ?? D.PredefinedType.Void;

                e.Result = ft.MakeArrayType().UnionWith(ft);
            }
        }

        private void Translator_VisitedAssignmentExpression(object sender, VisitedEventArgs<ICSharpCode.NRefactory.CSharp.AssignmentExpression> e)
        {
            if (e.Handled || e.Results.Count != 1)
            {
                return;
            }

            var ae = e.Results[0] as E.AssignmentExpression;

            if (ae == null)
            {
                return;
            }

            if (e.Node.Operator == AssignmentOperatorType.Add
                || e.Node.Operator == AssignmentOperatorType.Subtract)
            {
                var t = e.Context.ResoleClrType(e.Node.Left);

                if (t != null && typeof(MulticastDelegate).IsAssignableFrom(t))
                {
                    e.Results[0] = TranslateAssignmentExpression(ae);
                    e.Handled = true;
                }
            }
        }

        protected virtual Expression TranslateAssignmentExpression(E.AssignmentExpression expression)
        {
            if (expression.CompoundOperator == E.BinaryOperator.Add)
            {
                var array = expression.Target.LogicalOrWith(expression.Target.AssignedBy(new E.ArrayExpression()));

                return new E.ConditionalExpression()
                {
                    Condition = expression.Value,
                    TruePart = expression.Target.AssignedBy(array.Property("concat").Call(expression.Target)),
                    FalsePart = ExpressionBuilder.Undefined()
                };
            }
            else
            {
                var f = new E.FunctionExpression();
                var ifb = new S.IfStatement();
                ifb.Condition = expression.Target.LogicalAndWith(expression.Value);

                var inner = new S.IfStatement();
                inner.Condition = expression.Value.IsArray();

                inner.TruePart.Add(expression.Target.AssignedBy(expression.Target.Property("filter").Call(new E.ArrowFunctionExpression()
                {
                    Parameters = new Collection<D.Parameter>() { new D.Parameter("e") },
                    Statements = new Collection<Statement> {
                        expression.Value.Property("indexOf").Call(new E.IdentifierExpression("e")).IsLessThan(new E.NumberExpression(0)).ToReturn()
                    }
                })).ToStatement());

                inner.FalsePart.Add(expression.Target.Property("filter").Call(new E.ArrowFunctionExpression()
                {
                    Parameters = new Collection<D.Parameter>() { new D.Parameter("e") },
                    Statements = new Collection<Statement>()
                    {
                        new E.IdentifierExpression("e").IsStrictNotEqualTo(expression.Value).ToReturn()
                    }
                }).ToStatement());

                ifb.TruePart.Add(inner);

                f.Statements.Add(ifb);

                return f.Call();
            }
        }

        private void Translator_VisitedInvocationExpression(object sender, VisitedEventArgs<ICSharpCode.NRefactory.CSharp.InvocationExpression> e)
        {
            if (e.Handled || e.Results.Count != 1)
            {
                return;
            }

            var ce = e.Results[0] as E.CallExpression;

            if (ce == null)
            {
                return;
            }

            var t = e.Context.ResoleClrType(e.Node.Target);

            if (t != null && typeof(MulticastDelegate).IsAssignableFrom(t))
            {
                e.Results[0] = TranslateInvocationExpression(ce);
                e.Handled = true;
            }
        }

        protected virtual Expression TranslateInvocationExpression(E.CallExpression callExpression)
        {
            var f = new E.FunctionExpression();

            var l = new S.VariableDeclaration();
            l.Type = S.VariableDeclarationType.Let;
            l.Bindings.Add(new S.VariableBinding() { Variable = new E.IdentifierExpression("h") });

            f.Statements.Add(l);

            var ifb = new S.IfStatement();
            ifb.Condition = callExpression.Target;

            var inner = new S.IfStatement();
            inner.Condition = callExpression.Target.IsArray();

            inner.TruePart.Add(callExpression.Target.Property("forEach").Call(new E.ArrowFunctionExpression()
            {
                Parameters = new Collection<Declarations.Parameter>() {
                    new D.Parameter() { ParameterName="e" }
                },
                Statements = new Collection<Statement>()
                {
                    new E.IdentifierExpression("h").AssignedBy(new E.IdentifierExpression("e").Call(callExpression.Parameters)).ToStatement()
                }
            }).ToStatement());
            inner.TruePart.Add(new E.IdentifierExpression("h").ToReturn());

            inner.FalsePart.Add(callExpression.ToReturn());

            ifb.TruePart.Add(inner);
            f.Statements.Add(ifb);

            return f.Call();
        }
    }
}
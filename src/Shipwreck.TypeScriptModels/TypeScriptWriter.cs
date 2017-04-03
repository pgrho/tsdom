using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipwreck.TypeScriptModels.Declarations;
using Shipwreck.TypeScriptModels.Expressions;
using Shipwreck.TypeScriptModels.Statements;

namespace Shipwreck.TypeScriptModels
{
    public class TypeScriptWriter : IExpressionVisitor<int>, IStatementVistor<int>
    {
        private sealed class ClassMemberVisitor : IClassMemberVisitor<int>
        {
            private readonly TypeScriptWriter _Writer;

            private readonly bool _IsDeclare;

            public ClassMemberVisitor(TypeScriptWriter writer, bool isDeclare)
            {
                _Writer = writer;
                _IsDeclare = isDeclare;
            }

            int IClassMemberVisitor<int>.VisitConstructor(ConstructorDeclaration member)
            {
                _Writer.WriteConstructor(member, _IsDeclare);
                _Writer._Writer.WriteLine(';');
                return 0;
            }

            int IClassMemberVisitor<int>.VisitField(FieldDeclaration member)
            {
                _Writer.WriteField(member);
                _Writer._Writer.WriteLine(';');
                return 0;
            }

            int IClassMemberVisitor<int>.VisitMethod(MethodDeclaration member)
            {
                _Writer.WriteMethod(member, _IsDeclare);
                WriteLineIfNeeded();
                return 0;
            }

            int IClassMemberVisitor<int>.VisitGetAccessor(GetAccessorDeclaration member)
            {
                _Writer.WriteGetAccessor(member, _IsDeclare);
                WriteLineIfNeeded();
                return 0;
            }

            private void WriteLineIfNeeded()
            {
                if (_IsDeclare)
                {
                    _Writer._Writer.WriteLine(';');
                }
                else
                {
                    _Writer._Writer.WriteLine();
                }
            }

            int IClassMemberVisitor<int>.VisitSetAccessor(SetAccessorDeclaration member)
            {
                _Writer.WriteSetAccessor(member, _IsDeclare);
                WriteLineIfNeeded();
                return 0;
            }

            int IClassMemberVisitor<int>.VisitIndex(IndexSignature member)
            {
                member.WriteSignature(_Writer._Writer);
                _Writer._Writer.WriteLine(';');
                return 0;
            }
        }

        private sealed class InterfaceMemberVisitor : IInterfaceMemberVisitor<int>
        {
            private readonly TypeScriptWriter _Writer;
            private readonly bool _IsObjectLiteral;

            /// <summary>
            /// <see cref="InterfaceMemberVisitor" />クラスの新しいインスタンスを初期化します。
            /// </summary>
            public InterfaceMemberVisitor(TypeScriptWriter writer, bool isObjectLiteral)
            {
                _Writer = writer;
                _IsObjectLiteral = isObjectLiteral;
            }

            int IInterfaceMemberVisitor<int>.VisitField(FieldDeclaration member)
            {
                if (_IsObjectLiteral)
                {
                    _Writer.WriteFieldLiteral(member);
                }
                else
                {
                    _Writer.WriteField(member);
                }
                return 0;
            }

            int IInterfaceMemberVisitor<int>.VisitIndex(IndexSignature member)
            {
                member.WriteSignature(_Writer._Writer);
                return 0;
            }

            int IInterfaceMemberVisitor<int>.VisitMethod(MethodDeclaration member)
            {
                _Writer.WriteMethod(member, !_IsObjectLiteral);
                return 0;
            }

            int IInterfaceMemberVisitor<int>.VisitGetAccessor(GetAccessorDeclaration member)
            {
                _Writer.WriteGetAccessor(member, !_IsObjectLiteral);
                return 0;
            }

            int IInterfaceMemberVisitor<int>.VisitSetAccessor(SetAccessorDeclaration member)
            {
                _Writer.WriteSetAccessor(member, !_IsObjectLiteral);
                return 0;
            }
        }

        private readonly IndentedTextWriter _Writer;

        public TypeScriptWriter(TextWriter writer)
        {
            _Writer = (writer as IndentedTextWriter) ?? new IndentedTextWriter(writer);
        }

        #region IExpressionVisitor<int>

        // 4.2
        int IExpressionVisitor<int>.VisitThis()
        {
            _Writer.Write("this");
            return 0;
        }

        // 4.3
        int IExpressionVisitor<int>.VisitIdentifier(IdentifierExpression expression)
        {
            _Writer.Write(expression.Name);
            return 0;
        }

        // 4.4
        int IExpressionVisitor<int>.VisitNull()
        {
            _Writer.Write("null");
            return 0;
        }

        int IExpressionVisitor<int>.VisitBoolean(BooleanExpression expression)
        {
            _Writer.Write(expression.Value ? "true" : "false");
            return 0;
        }

        int IExpressionVisitor<int>.VisitNumber(NumberExpression expression)
        {
            //todo:
            _Writer.Write(expression.Value);
            return 0;
        }

        int IExpressionVisitor<int>.VisitString(StringExpression expression)
        {
            _Writer.WriteLiteral(expression.Value);
            return 0;
        }

        int IExpressionVisitor<int>.VisitRegExp(RegExpExpression expression)
        {
            _Writer.Write('/');
            _Writer.Write(expression.Pattern);
            _Writer.Write('/');
            _Writer.Write(expression.Option);
            return 0;
        }

        // 4.5
        int IExpressionVisitor<int>.VisitObject(ObjectExpression expression)
        {
            _Writer.WriteLine('{');

            if (expression.HasMember)
            {
                _Writer.Indent++;
                var cv = new InterfaceMemberVisitor(this, true);
                for (var i = 0; i < expression.Members.Count; i++)
                {
                    expression.Members[i].Accept(cv);
                    if (i < expression.Members.Count - 1)
                    {
                        _Writer.Write(',');
                    }
                    _Writer.WriteLine();
                }
                _Writer.Indent--;
            }
            _Writer.Write('}');

            return 0;
        }

        // 4.6
        int IExpressionVisitor<int>.VisitArray(ArrayExpression expression)
        {
            _Writer.WriteLine('[');

            if (expression.HasElement)
            {
                _Writer.Indent++;
                for (var i = 0; i < expression.Elements.Count; i++)
                {
                    expression.Elements[i].Accept(this);
                    if (i < expression.Elements.Count - 1)
                    {
                        _Writer.Write(',');
                    }
                }
                _Writer.Indent--;
            }
            _Writer.Write(']');
            return 0;
        }

        // TODO:4.7

        // 4.8
        int IExpressionVisitor<int>.VisitParentheses(ParenthesesExpression expression)
        {
            _Writer.Write('(');
            expression.Expression.Accept(this);
            _Writer.Write(')');

            return 0;
        }

        // 4.9
        int IExpressionVisitor<int>.VisitSuper()
        {
            _Writer.Write("super");
            return 0;
        }

        // 4.10
        int IExpressionVisitor<int>.VisitFunction(FunctionExpression expression)
        {
            _Writer.Write("function");
            if (!string.IsNullOrEmpty(expression.FunctionName))
            {
                _Writer.Write(' ');
                _Writer.Write(expression.FunctionName);
            }
            _Writer.WriteCallSignature(expression, this);

            if (expression.HasStatement)
            {
                WriteMethodBody(expression.Statements);
            }
            else
            {
                _Writer.Write(" {}");
            }
            return 0;
        }

        // 4.11
        int IExpressionVisitor<int>.VisitArrowFunction(ArrowFunctionExpression expression)
        {
            if (expression.HasParameter)
            {
                _Writer.WriteParameters(expression.Parameters, this);
            }
            else
            {
                _Writer.Write("()");
            }
            _Writer.Write(" => ");

            if (expression.HasStatement && expression.Statements.Count == 1 && expression.Statements[0] is ReturnStatement)
            {
                var rs = ((ReturnStatement)expression.Statements[0]).Value;

                if (rs != null)
                {
                    if (rs is ObjectExpression)
                    {
                        _Writer.Write('(');
                        rs.WriteExpression(_Writer);
                        _Writer.Write(')');
                    }
                    else
                    {
                        rs.WriteExpression(_Writer);
                    }

                    return 0;
                }
            }

            if (expression.HasStatement)
            {
                WriteMethodBody(expression.Statements);
            }
            else
            {
                _Writer.Write(" {}");
            }

            return 0;
        }

        // 4.13
        int IExpressionVisitor<int>.VisitProperty(PropertyExpression property)
        {
            property.Accept(this);
            if (property.IsValidIdentifier)
            {
                _Writer.Write('.');
                _Writer.Write(property.Property);
            }
            else
            {
                _Writer.Write('[');
                _Writer.WriteLiteral(property.Property);
                _Writer.Write(']');
            }
            return 0;
        }

        // 4.14
        int IExpressionVisitor<int>.VisitNew(NewExpression expression)
        {
            _Writer.Write("new ");
            WriteChildExpression(expression, expression.Type);
            if (expression.HasTypeArgument)
            {
                _Writer.WriteTypeParameters(expression.TypeArguments);
            }
            if (expression.HasParameter)
            {
                _Writer.WriteParameters(expression.Parameters, this);
            }
            else
            {
                _Writer.Write("()");
            }
            return 0;
        }

        // 4.15
        int IExpressionVisitor<int>.VisitCall(CallExpression expression)
        {
            WriteChildExpression(expression, expression.Type);
            if (expression.HasTypeArgument)
            {
                _Writer.WriteTypeParameters(expression.TypeArguments);
            }
            if (expression.HasParameter)
            {
                _Writer.WriteParameters(expression.Parameters, this);
            }
            else
            {
                _Writer.Write("()");
            }
            return 0;
        }

        // 4.16
        int IExpressionVisitor<int>.VisitTypeAssertion(TypeAssertionExpression expression)
        {
            _Writer.Write('<');
            expression.Type.Accept(this);
            _Writer.Write('>');
            WriteChildExpression(expression, expression.Operand);

            return 0;
        }

        // 4.18
        int IExpressionVisitor<int>.VisitUnary(UnaryExpression expression)
        {
            switch (expression.Operator)
            {
                case UnaryOperator.PrefixIncrement:
                    _Writer.Write("++");
                    break;

                case UnaryOperator.PrefixDecrement:
                    _Writer.Write("--");
                    break;

                case UnaryOperator.PostfixIncrement:
                case UnaryOperator.PostfixDecrement:
                    break;

                case UnaryOperator.Plus:
                    _Writer.Write('+');
                    break;

                case UnaryOperator.Minus:
                    _Writer.Write('-');
                    break;

                case UnaryOperator.BitwiseNot:
                    _Writer.Write('~');
                    break;

                case UnaryOperator.LogicalNot:
                    _Writer.Write('!');
                    break;

                case UnaryOperator.Delete:
                    _Writer.Write("delete ");
                    break;

                case UnaryOperator.Void:
                    _Writer.Write("void ");
                    break;

                case UnaryOperator.TypeOf:
                    _Writer.Write("typeof ");
                    break;

                default:
                    throw new NotImplementedException($"{nameof(UnaryOperator)}.{expression.Operator}");
            }

            WriteChildExpression(expression, expression.Operand);

            switch (expression.Operator)
            {
                case UnaryOperator.PostfixIncrement:
                    _Writer.Write("++");
                    break;

                case UnaryOperator.PostfixDecrement:
                    _Writer.Write("--");
                    break;
            }

            return 0;
        }

        // 4.19
        int IExpressionVisitor<int>.VisitBinary(BinaryExpression expression)
        {
            WriteChildExpression(expression, expression.Left);
            _Writer.Write(' ');
            _Writer.Write(expression.Operator.GetToken());
            _Writer.Write(' ');
            WriteChildExpression(expression, expression.Right);

            return 0;
        }

        // 4.20
        int IExpressionVisitor<int>.VisitConditional(ConditionalExpression expression)
        {
            WriteChildExpression(expression, expression.Condition);
            _Writer.Write(" ? ");
            WriteChildExpression(expression, expression.TruePart);
            _Writer.Write(" : ");
            WriteChildExpression(expression, expression.FalsePart);
            return 0;
        }

        // 4.21
        int IExpressionVisitor<int>.VisitAssignment(AssignmentExpression expression)
        {
            expression.Target.Accept(this);

            _Writer.Write(' ');
            switch (expression.CompoundOperator)
            {
                case BinaryOperator.Default:
                    break;

                case BinaryOperator.Add:
                case BinaryOperator.Subtract:
                case BinaryOperator.Multiply:
                case BinaryOperator.Divide:
                case BinaryOperator.IntegerDivide:
                case BinaryOperator.LeftShift:
                case BinaryOperator.SignedRightShift:
                case BinaryOperator.UnsignedRightShift:
                case BinaryOperator.BitwiseAnd:
                case BinaryOperator.BitwiseOr:
                case BinaryOperator.BitwiseXor:
                    _Writer.Write(expression.CompoundOperator.GetToken());
                    break;

                default:
                    throw new NotImplementedException($"{nameof(BinaryOperator)}.{expression.CompoundOperator}");
            }
            _Writer.Write("= ");

            expression.Value.Accept(this);

            return 0;
        }

        // 4.22
        int IExpressionVisitor<int>.VisitComma(CommaExpression expression)
        {
            WriteChildExpression(expression, expression.Left);
            _Writer.Write(", ");
            WriteChildExpression(expression, expression.Right);

            return 0;
        }

        private void WriteChildExpression(Expression parent, Expression child)
        {
            // TODO: Determine precedence
            _Writer.Write('(');
            child.Accept(this);
            _Writer.Write(')');
        }

        #endregion IExpressionVisitor<int>

        #region IStatementVisitor<int>

        // 5.2
        int IStatementVistor<int>.VisitVariableDeclaration(VariableDeclaration statement)
        {
            if (statement.HasBinding)
            {
                _Writer.Write("var ");

                WriteBindings(statement.Bindings);
            }

            return 0;
        }

        // 5.3
        int IStatementVistor<int>.VisitLetDeclaration(LetDeclaration statement)
        {
            if (statement.HasBinding)
            {
                _Writer.Write("let ");

                WriteBindings(statement.Bindings);
            }

            return 0;
        }

        // 5.3
        int IStatementVistor<int>.VisitConstDeclaration(ConstDeclaration statement)
        {
            if (statement.HasBinding)
            {
                _Writer.Write("const ");

                WriteBindings(statement.Bindings);
            }

            return 0;
        }

        private void WriteBindings(Collection<VariableBinding> bindings)
        {
            for (var i = 0; i < bindings.Count; i++)
            {
                if (i > 0)
                {
                    _Writer.Write(", ");
                }

                var b = bindings[0];

                b.Variable.Accept(this);

                if (b.Initializer != null)
                {
                    _Writer.Write(" = ");
                    b.Initializer.Accept(this);
                }
            }
            _Writer.WriteLine(';');
        }

        // 5.4
        int IStatementVistor<int>.VisitIf(IfStatement statement)
        {
            _Writer.Write("if (");
            statement.Condition.Accept(this);
            _Writer.WriteLine(") {");

            _Writer.Indent++;

            if (statement.HasTruePart)
            {
                foreach (var s in statement.TruePart)
                {
                    s.Accept(this);
                }
            }

            _Writer.Indent--;

            if (statement.HasFalsePart)
            {
                if (statement.FalsePart.Count == 0 && statement.FalsePart[0] is IfStatement)
                {
                    _Writer.Write("} else ");

                    return statement.FalsePart[0].Accept(this);
                }
                else
                {
                    _Writer.WriteLine("} else {");
                    _Writer.Indent++;

                    if (statement.HasTruePart)
                    {
                        foreach (var s in statement.TruePart)
                        {
                            s.Accept(this);
                        }
                    }

                    _Writer.Indent--;
                }
            }
            _Writer.WriteLine('}');

            return 0;
        }

        // 5.4
        int IStatementVistor<int>.VisitDo(DoStatement statement)
        {
            _Writer.WriteLine("do {");

            if (statement.HasStatement)
            {
                _Writer.Indent++;
                foreach (var s in statement.Statements)
                {
                    s.Accept(this);
                }
                _Writer.Indent--;
            }

            _Writer.Write("} while (");
            statement.Condition.Accept(this);
            _Writer.WriteLine(");");

            return 0;
        }

        // 5.4
        int IStatementVistor<int>.VisitWhile(WhileStatement statement)
        {
            _Writer.Write("while (");
            statement.Condition.Accept(this);
            _Writer.WriteLine(") {");

            if (statement.HasStatement)
            {
                _Writer.Indent++;
                foreach (var s in statement.Statements)
                {
                    s.Accept(this);
                }
                _Writer.Indent--;
            }

            _Writer.WriteLine('}');

            return 0;
        }

        // 5.5
        int IStatementVistor<int>.VisitFor(ForStatement statement)
        {
            _Writer.Write("for (");
            WriteForInitializer(statement.Initializer);
            _Writer.WriteLine("; ");
            statement.Condition?.Accept(this);
            _Writer.WriteLine("; ");
            statement.Increment?.Accept(this);
            _Writer.WriteLine(") {");

            if (statement.HasStatement)
            {
                _Writer.Indent++;
                foreach (var s in statement.Statements)
                {
                    s.Accept(this);
                }
                _Writer.Indent--;
            }

            _Writer.WriteLine('}');

            return 0;
        }

        private void WriteForInitializer(Expression initializer)
        {
            var v = initializer as ForBindingExpression;
            if (v != null)
            {
                _Writer.Write("var ");
                v.Variable.Accept(this);
                if (v.Variable != null)
                {
                    _Writer.Write(" = ");
                    v.Variable.Accept(this);
                }
            }
            else
            {
                initializer?.Accept(this);
            }
        }

        // 5.6
        int IStatementVistor<int>.VisitForIn(ForInStatement statement)
        {
            _Writer.Write("for (");
            WriteForInitializer(statement.Variable);
            _Writer.WriteLine(" in ");
            statement.Value.Accept(this);
            _Writer.WriteLine(") {");

            if (statement.HasStatement)
            {
                _Writer.Indent++;
                foreach (var s in statement.Statements)
                {
                    s.Accept(this);
                }
                _Writer.Indent--;
            }

            _Writer.WriteLine('}');

            return 0;
        }

        // 5.7
        int IStatementVistor<int>.VisitForOf(ForOfStatement statement)
        {
            _Writer.Write("for (");
            WriteForInitializer(statement.Variable);
            _Writer.WriteLine(" of ");
            statement.Value.Accept(this);
            _Writer.WriteLine(") {");

            if (statement.HasStatement)
            {
                _Writer.Indent++;
                foreach (var s in statement.Statements)
                {
                    s.Accept(this);
                }
                _Writer.Indent--;
            }

            _Writer.WriteLine('}');

            return 0;
        }

        // 5.8
        int IStatementVistor<int>.VisitContinue(ContinueStatement statement)
        {
            _Writer.WriteLine("continue;");
            return 0;
        }

        // 5.9
        int IStatementVistor<int>.VisitBreak(BreakStatement statement)
        {
            _Writer.WriteLine("break;");
            return 0;
        }

        // 5.10
        int IStatementVistor<int>.VisitReturn(ReturnStatement statement)
        {
            _Writer.Write("return");
            if (statement.Value != null)
            {
                _Writer.Write(' ');
                statement.Value.Accept(this);
            }
            _Writer.WriteLine(';');
            return 0;
        }

        // 5.11
        int IStatementVistor<int>.VisitWith(WithStatement statement)
        {
            throw new NotImplementedException("WithStatement is not supported in TypeScript.");
        }

        // 5.12
        int IStatementVistor<int>.VisitSwitch(SwitchStatement statement)
        {
            _Writer.Write("swtch (");
            statement.Condition.Accept(this);
            _Writer.WriteLine(") {");

            if (statement.HasCase)
            {
                _Writer.Indent++;
                foreach (var c in statement.Cases)
                {
                    if (c.Label == null)
                    {
                        _Writer.Write("default");
                    }
                    else
                    {
                        _Writer.Write("case ");
                        c.Label.Accept(this);
                    }
                    _Writer.WriteLine(':');
                    if (c.HasStatement)
                    {
                        _Writer.Indent++;
                        foreach (var s in c.Statements)
                        {
                            s.Accept(this);
                        }
                        _Writer.Indent--;
                    }
                }
                _Writer.Indent--;
            }

            _Writer.WriteLine('}');

            return 0;
        }

        // 5.13
        int IStatementVistor<int>.VisitThrow(ThrowStatement statement)
        {
            _Writer.Write("throw ");
            statement.Value.Accept(this);
            _Writer.WriteLine(';');

            return 0;
        }

        // 5.14
        int IStatementVistor<int>.VisitTry(TryStatement statement)
        {
            _Writer.WriteLine("try {");

            if (statement.HasTryBlock)
            {
                _Writer.Indent++;
                foreach (var s in statement.TryBlock)
                {
                    s.Accept(this);
                }
                _Writer.Indent--;
            }

            if (statement.HasCatchBlock)
            {
                _Writer.Write("} catch(");
                statement.CatchParameter.Accept(this);
                _Writer.WriteLine(") {");

                _Writer.Indent++;
                foreach (var s in statement.CatchBlock)
                {
                    s.Accept(this);
                }
                _Writer.Indent--;

                if (statement.HasFinallyBlock)
                {
                    _Writer.WriteLine("} finally {");

                    _Writer.Indent++;
                    foreach (var s in statement.FinallyBlock)
                    {
                        s.Accept(this);
                    }
                    _Writer.Indent--;
                }
                _Writer.WriteLine('}');
            }
            else
            {
                _Writer.WriteLine("} finally {");
                if (statement.HasFinallyBlock)
                {
                    _Writer.Indent++;
                    foreach (var s in statement.FinallyBlock)
                    {
                        s.Accept(this);
                    }
                    _Writer.Indent--;
                }
                _Writer.WriteLine('}');
            }

            return 0;
        }

        #endregion IStatementVisitor<int>

        #region Declarations

        #region 6.1 Function Declarations

        private int VisitFunction(FunctionDeclaration member)
        {
            if (member.HasOverload)
            {
                foreach (var ov in member.Overloads)
                {
                    WriteFunctionSignature(member, ov);
                    _Writer.WriteLine();
                }
            }
            WriteFunctionSignature(member);

            if (member.IsDeclare)
            {
                _Writer.WriteLine(';');
            }
            else if (member.HasStatement)
            {
                WriteMethodBody(member.Statements);
            }
            else
            {
                _Writer.WriteLine(" {");
                _Writer.WriteLine('}');
            }

            return 0;
        }

        private void WriteFunctionSignature(FunctionDeclaration member, ICallSignature signature = null)
        {
            if (member.IsDeclare)
            {
                _Writer.Write("declare ");
            }
            if (member.IsExport)
            {
                _Writer.Write("export ");
            }
            if (member.IsDeclare)
            {
                _Writer.Write("default ");
            }
            _Writer.Write("function ");
            if (member.FunctionName != null)
            {
                _Writer.Write(' ');
                _Writer.Write(member.FunctionName);
            }
            _Writer.WriteCallSignature(signature ?? member, this);
        }

        #endregion 6.1 Function Declarations

        // 7.1
        private int VisitInterface(InterfaceDeclaration member)
        {
            if (member.IsDeclare)
            {
                _Writer.Write("declare ");
            }
            if (member.IsExport)
            {
                _Writer.Write("export ");
            }
            _Writer.Write("interface ");
            _Writer.Write(member.Name);

            if (member.HasTypeParameter)
            {
                _Writer.WriteTypeParameters(member.TypeParameters);
            }
            if (member.HasBaseType)
            {
                for (int i = 0; i < member.BaseTypes.Count; i++)
                {
                    _Writer.Write(i == 0 ? " : " : ", ");
                    member.BaseTypes[i].WriteTypeReference(_Writer);
                }
            }

            _Writer.WriteLine(" {");

            if (member.HasMember)
            {
                _Writer.Indent++;
                var cv = new InterfaceMemberVisitor(this, false);
                for (var i = 0; i < member.Members.Count; i++)
                {
                    member.Members[i].Accept(cv);
                    _Writer.WriteLine(';');
                }
                _Writer.Indent--;
            }
            _Writer.Write('}');

            return 0;
        }

        // 8.1
        private int VisitClassDeclaration(ClassDeclaration member)
        {
            if (member.IsDeclare)
            {
                _Writer.Write("declare ");
            }
            if (member.IsExport)
            {
                _Writer.Write("export ");
            }
            if (member.IsDefault)
            {
                _Writer.Write("default ");
            }
            _Writer.Write("class ");
            _Writer.Write(member.Name);

            if (member.HasTypeParameter)
            {
                _Writer.WriteTypeParameters(member.TypeParameters);
            }

            if (member.BaseType != null)
            {
                _Writer.Write(" extends ");
                member.BaseType.WriteTypeReference(_Writer);
            }

            if (member.HasInterface)
            {
                for (int i = 0; i < member.Interface.Count; i++)
                {
                    _Writer.Write(i == 0 ? " implements " : ", ");
                    member.Interface[i].WriteTypeReference(_Writer);
                }
            }

            _Writer.WriteLine(" {");

            if (member.HasMember)
            {
                _Writer.Indent++;
                var cv = new ClassMemberVisitor(this, member.IsDeclare);
                for (var i = 0; i < member.Members.Count; i++)
                {
                    member.Members[i].Accept(cv);
                    _Writer.WriteLine(';');
                }
                _Writer.Indent--;
            }
            _Writer.Write('}');

            return 0;
        }

        // 9.1
        private int VisitEnumDeclaration(EnumDeclaration member)
        {
            if (member.IsDeclare)
            {
                _Writer.Write("declare ");
            }
            if (member.IsExport)
            {
                _Writer.Write("export ");
            }
            if (member.IsConst)
            {
                _Writer.Write("const ");
            }
            _Writer.Write("enum ");
            _Writer.Write(member.Name);
            _Writer.WriteLine(" {");
            if (member.HasMember)
            {
                _Writer.Indent++;
                for (var i = 0; i < member.Members.Count; i++)
                {
                    var m = member.Members[i];
                    _Writer.Write(m.FieldName);
                    if (m.Initializer != null)
                    {
                        _Writer.Write(" = ");
                        m.Initializer.Accept(this);
                    }
                    if (i < member.Members.Count - 1)
                    {
                        _Writer.WriteLine(',');
                    }
                    else
                    {
                        _Writer.WriteLine();
                    }
                }
                _Writer.Indent--;
            }
            _Writer.WriteLine('}');

            return 0;
        }

        #region Member

        // TODO: 6.4 Destructuring Parameter Declarations

        // TODO: 6.7 Generator Functions

        // TODO: 6.8 Asynchronous Functions

        // TODO: 6.9 Type Guard Functions

        #region WriteField

        private void WriteField(FieldDeclaration member)
        {
            _Writer.WriteAccessibility(member.Accessibility);
            if (member.IsStatic)
            {
                _Writer.Write("static ");
            }
            _Writer.Write(member.FieldName);
            if (member.FieldType != null)
            {
                _Writer.Write(": ");
                member.FieldType.WriteTypeReference(_Writer);
            }
            if (member.Initializer != null)
            {
                _Writer.Write(" = ");
                member.Initializer.Accept(this);
            }
            _Writer.WriteLine(';');
        }

        private void WriteFieldLiteral(FieldDeclaration member)
        {
            _Writer.Write(member.FieldName);
            if (member.Initializer != null)
            {
                _Writer.Write(": ");
                member.Initializer.Accept(this);
            }
        }

        #endregion WriteField

        #region WriteConstructor

        private void WriteConstructor(ConstructorDeclaration member, bool skipBody)
        {
            if (member.HasOverload)
            {
                foreach (var ov in member.Overloads)
                {
                    WriteConstructorSignature(member, ov);
                    _Writer.WriteLine();
                }
            }
            WriteConstructorSignature(member);

            if (skipBody)
            {
                _Writer.WriteLine(';');
            }
            else if (member.HasStatement)
            {
                WriteMethodBody(member.Statements);
                _Writer.WriteLine(';');
            }
            else
            {
                _Writer.WriteLine(" {");
                _Writer.WriteLine('}');
            }
        }

        private void WriteConstructorSignature(ConstructorDeclaration member, ICallSignature signature = null)
        {
            _Writer.WriteAccessibility(member.Accessibility);
            _Writer.Write("function ");
            _Writer.WriteCallSignature(signature ?? member, this);
        }

        #endregion WriteConstructor

        #region WriteGetAccessor/WriteSetAccessor

        private void WriteGetAccessor(AccessorDeclaration member, bool skipBody)
        {
            _Writer.WriteAccessibility(member.Accessibility);
            if (member.IsStatic)
            {
                _Writer.Write("static ");
            }
            _Writer.Write(member.PropertyName);
            _Writer.Write(" get()");
            if (member.PropertyType != null)
            {
                _Writer.Write(": ");
                member.PropertyType.WriteTypeReference(_Writer);
            }

            if (skipBody)
            {
            }
            else if (member.HasStatement)
            {
                WriteMethodBody(member.Statements);
                _Writer.WriteLine();
            }
            else
            {
                _Writer.WriteLine(" {");
                _Writer.WriteLine('}');
            }
        }

        private void WriteSetAccessor(SetAccessorDeclaration member, bool skipBody)
        {
            _Writer.WriteAccessibility(member.Accessibility);
            if (member.IsStatic)
            {
                _Writer.Write("static ");
            }
            _Writer.Write(member.PropertyName);
            _Writer.Write(" set(");
            _Writer.Write(member.ParameterName);
            _Writer.Write(": ");
            member.PropertyType.WriteTypeReference(_Writer);
            _Writer.Write(')');

            if (member.HasStatement)
            {
                WriteMethodBody(member.Statements);
                _Writer.WriteLine();
            }
            else
            {
                _Writer.WriteLine(" {");
                _Writer.WriteLine('}');
            }
        }

        #endregion WriteGetAccessor/WriteSetAccessor

        #region WriteMethod

        private void WriteMethod(MethodDeclaration member, bool skipBody)
        {
            if (member.HasOverload)
            {
                foreach (var ov in member.Overloads)
                {
                    WriteMethodSignature(member, ov);
                    _Writer.WriteLine();
                }
            }
            WriteMethodSignature(member);

            if (skipBody)
            {
            }
            else if (member.HasStatement)
            {
                WriteMethodBody(member.Statements);
                _Writer.WriteLine();
            }
            else
            {
                _Writer.WriteLine(" {");
                _Writer.WriteLine('}');
            }
        }

        private void WriteMethodSignature(MethodDeclaration member, ICallSignature signature = null)
        {
            _Writer.WriteAccessibility(member.Accessibility);
            if (member.IsStatic)
            {
                _Writer.Write("static ");
            }
            _Writer.Write(member.MethodName);
            _Writer.WriteCallSignature(signature ?? member, this);
        }

        #endregion WriteMethod

        private void WriteMethodBody(Collection<Statement> statements)
        {
            _Writer.WriteLine(" {");
            _Writer.Indent++;

            if (statements?.Count > 0)
            {
                foreach (var s in statements)
                {
                    s.Accept(this);
                }
            }

            _Writer.Indent--;
            _Writer.Write('}');
        }

        #endregion Member

        #endregion Declarations
    }
}
using Shipwreck.TypeScriptModels.Declarations;
using Shipwreck.TypeScriptModels.Expressions;
using Shipwreck.TypeScriptModels.Statements;
using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.IO;

namespace Shipwreck.TypeScriptModels
{
    public class TypeScriptWriter : IDisposable, IExpressionVisitor<int>, IStatementVisitor<int>, IModuleMemberVisitor<int>, INamespaceMemberVisitor<int>, IRootStatementVisitor<int>
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
                return 0;
            }

            int IClassMemberVisitor<int>.VisitField(FieldDeclaration member)
            {
                _Writer.WriteField(member);
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
            /// <see cref="InterfaceMemberVisitor" /> クラスの新しいインスタンスを初期化します。
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
        private readonly TextWriter _BaseWriter;
        private readonly bool _LeaveOpen;

        public TypeScriptWriter(TextWriter writer, bool leaveOpen = false)
        {
            _Writer = writer as IndentedTextWriter ?? new IndentedTextWriter(writer);
            _LeaveOpen = leaveOpen;
            _BaseWriter = writer;
        }

        public void Flush()
        {
            _Writer.Flush();
            _BaseWriter.Flush();
        }

        public void Write(Syntax syntax)
        {
            var rs = syntax as IRootStatement;
            if (rs != null)
            {
                rs.Accept(this);
                return;
            }

            var ns = syntax as INamespaceMember;
            if (ns != null)
            {
                ns.Accept(this);
                return;
            }

            var ms = syntax as IModuleMember;
            if (ms != null)
            {
                ms.Accept(this);
                return;
            }
            var cl = syntax as IClassMember;
            if (cl != null)
            {
                cl.Accept(new ClassMemberVisitor(this, false));
                return;
            }

            var inf = syntax as IInterfaceMember;
            if (inf != null)
            {
                inf.Accept(new InterfaceMemberVisitor(this, false));
                return;
            }

            var s = syntax as Statement;
            if (s != null)
            {
                s.Accept(this);
            }
            else
            {
                ((Expression)syntax).Accept(this);
            }
        }

        /// <summary>
        /// インスタンスが破棄されているかどうかを示す値を取得します。
        /// </summary>
        protected bool IsDisposed { get; private set; }

        #region IDisposable メソッド

        /// <summary>
        /// アンマネージ リソースの解放およびリセットに関連付けられているアプリケーション定義のタスクを実行します。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable メソッド

        #region デストラクタ

        /// <summary>
        /// オブジェクトがガベジ コレクションにより収集される前に、そのオブジェクトがリソースを解放し、その他のクリーンアップ操作を実行できるようにします。
        /// </summary>
        ~TypeScriptWriter()
        {
            Dispose(false);
        }

        #endregion デストラクタ

        #region 仮想メソッド

        /// <summary>
        /// アンマネージ リソースの解放およびリセットに関連付けられているアプリケーション定義のタスクを実行します。
        /// </summary>
        /// <param name="disposing">
        /// メソッドが <see cref="TypeScriptWriter.Dispose()" /> から呼び出された場合は <c>true</c>。その他の場合は <c>false</c>。
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;

            if (disposing)
            {
                if (_Writer != _BaseWriter)
                {
                    _Writer.Dispose();
                }
                if (!_LeaveOpen)
                {
                    _BaseWriter.Dispose();
                }
            }
        }

        #endregion 仮想メソッド

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
            WriteCallSignature(expression);

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
                WriteParameters(expression.Parameters);
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
        int IExpressionVisitor<int>.VisitProperty(PropertyExpression expression)
        {
            expression.Object.Accept(this);
            if (expression.IsValidIdentifier)
            {
                _Writer.Write('.');
                _Writer.Write(expression.Property);
            }
            else
            {
                _Writer.Write('[');
                _Writer.WriteLiteral(expression.Property);
                _Writer.Write(']');
            }
            return 0;
        }

        int IExpressionVisitor<int>.VisitIndexer(IndexerExpression expression)
        {
            expression.Object.Accept(this);
            _Writer.Write('[');
            expression.Index.Accept(this);
            _Writer.Write(']');

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
            WriteChildExpression(expression, expression.Target);
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
            var pp = parent.Precedence;
            var cp = child.Precedence;
            if (pp != ExpressionPrecedence.Unknown && cp != ExpressionPrecedence.Unknown && cp > pp)
            {
                child.Accept(this);
            }
            else
            {
                _Writer.Write('(');
                child.Accept(this);
                _Writer.Write(')');
            }
        }

        #endregion IExpressionVisitor<int>

        #region IStatementVisitor<int>

        // 5.2
        int IStatementVisitor<int>.VisitVariableDeclaration(VariableDeclaration statement)
        {
            if (statement.HasBinding)
            {
                WriteIsDeclare(statement.IsDeclare);
                WriteIsExport(statement.IsExport);
                switch (statement.Type)
                {
                    case VariableDeclarationType.Var:
                        _Writer.Write("var ");
                        break;

                    case VariableDeclarationType.Let:
                        _Writer.Write("let ");
                        break;

                    case VariableDeclarationType.Const:
                        _Writer.Write("const ");
                        break;

                    default:
                        throw new NotImplementedException($"{nameof(VariableDeclarationType)}.{statement.Type}");
                }

                for (var i = 0; i < statement.Bindings.Count; i++)
                {
                    if (i > 0)
                    {
                        _Writer.Write(", ");
                    }

                    var b = statement.Bindings[i];

                    b.Variable.Accept(this);

                    if (b.Type != null)
                    {
                        _Writer.Write(" : ");
                        b.Type.WriteTypeReference(_Writer);
                    }

                    if (b.Initializer != null)
                    {
                        _Writer.Write(" = ");
                        b.Initializer.Accept(this);
                    }
                }
                _Writer.WriteLine(';');
            }

            return 0;
        }

        // 5.4
        int IStatementVisitor<int>.VisitIf(IfStatement statement)
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
        int IStatementVisitor<int>.VisitDo(DoStatement statement)
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
        int IStatementVisitor<int>.VisitWhile(WhileStatement statement)
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
        int IStatementVisitor<int>.VisitFor(ForStatement statement)
        {
            _Writer.Write("for (");
            WriteForInitializer(statement.Initializer);
            _Writer.Write("; ");
            statement.Condition?.Accept(this);
            _Writer.Write("; ");
            statement.Iterator?.Accept(this);
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
                if (v.Initializer != null)
                {
                    _Writer.Write(" = ");
                    v.Initializer.Accept(this);
                }
            }
            else
            {
                initializer?.Accept(this);
            }
        }

        // 5.6
        int IStatementVisitor<int>.VisitForIn(ForInStatement statement)
        {
            _Writer.Write("for (");
            WriteForInitializer(statement.Variable);
            _Writer.Write(" in ");
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
        int IStatementVisitor<int>.VisitForOf(ForOfStatement statement)
        {
            _Writer.Write("for (");
            WriteForInitializer(statement.Variable);
            _Writer.Write(" of ");
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
        int IStatementVisitor<int>.VisitContinue(ContinueStatement statement)
        {
            _Writer.WriteLine("continue;");
            return 0;
        }

        // 5.9
        int IStatementVisitor<int>.VisitBreak(BreakStatement statement)
        {
            _Writer.WriteLine("break;");
            return 0;
        }

        // 5.10
        int IStatementVisitor<int>.VisitReturn(ReturnStatement statement)
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
        int IStatementVisitor<int>.VisitWith(WithStatement statement)
        {
            throw new NotImplementedException("WithStatement is not supported in TypeScript.");
        }

        // 5.12
        int IStatementVisitor<int>.VisitSwitch(SwitchStatement statement)
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
        int IStatementVisitor<int>.VisitThrow(ThrowStatement statement)
        {
            _Writer.Write("throw ");
            statement.Value.Accept(this);
            _Writer.WriteLine(';');

            return 0;
        }

        // 5.14
        int IStatementVisitor<int>.VisitTry(TryStatement statement)
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

        // 定義なし
        int IStatementVisitor<int>.VisitExpression(ExpressionStatement statement)
        {
            if (statement.Expression != null)
            {
                statement.Expression.Accept(this);
            }
            _Writer.WriteLine(';');
            return 0;
        }

        int IStatementVisitor<int>.VisitBlock(BlockStatement statement)
        {
            if (statement.HasStatement)
            {
                _Writer.WriteLine('{');
                _Writer.Indent++;
                foreach (var s in statement.Statements)
                {
                    s.Accept(this);
                }
                _Writer.Indent--;
                _Writer.WriteLine('}');
            }

            return 0;
        }

        #endregion IStatementVisitor<int>

        private void WriteIsDeclare(bool isDeclare)
        {
            if (isDeclare)
            {
                _Writer.Write("declare ");
            }
        }

        private void WriteIsExport(bool isExport)
        {
            if (isExport)
            {
                _Writer.Write("export ");
            }
        }

        private void WriteIsDefault(bool isExport)
        {
            if (isExport)
            {
                _Writer.Write("default ");
            }
        }

        private void WriteIsStatic(bool isStatic)
        {
            if (isStatic)
            {
                _Writer.Write("static ");
            }
        }

        private void WriteIsAbstract(bool isStatic)
        {
            if (isStatic)
            {
                _Writer.Write("abstract ");
            }
        }

        #region Declarations

        #region 6.1 Function Declarations

        private int VisitFunction(FunctionDeclaration member)
        {
            // TODO:
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
            WriteIsDeclare(member.IsDeclare);
            WriteIsExport(member.IsExport);
            WriteIsDefault(member.IsDefault);
            _Writer.Write("function ");
            if (member.FunctionName != null)
            {
                _Writer.Write(' ');
                _Writer.Write(member.FunctionName);
            }
            WriteCallSignature(signature ?? member);
        }

        #endregion 6.1 Function Declarations

        #region Module Member

        // 7.1

        int IRootStatementVisitor<int>.VisitInterfaceDeclaration(InterfaceDeclaration declaration)
        {
            WriteInterfaceDeclaration(declaration);
            _Writer.WriteLine();

            return 0;
        }

        int IModuleMemberVisitor<int>.VisitInterfaceDeclaration(InterfaceDeclaration declaration)
        {
            WriteInterfaceDeclaration(declaration);
            _Writer.WriteLine();

            return 0;
        }

        int INamespaceMemberVisitor<int>.VisitInterfaceDeclaration(InterfaceDeclaration declaration)
        {
            WriteInterfaceDeclaration(declaration);
            _Writer.WriteLine();

            return 0;
        }

        private int WriteInterfaceDeclaration(InterfaceDeclaration declaration)
        {
            WriteIsDeclare(declaration.IsDeclare);
            WriteIsExport(declaration.IsExport);
            _Writer.Write("interface ");
            _Writer.Write(declaration.Name);

            if (declaration.HasTypeParameter)
            {
                _Writer.WriteTypeParameters(declaration.TypeParameters);
            }
            if (declaration.HasBaseType)
            {
                for (int i = 0; i < declaration.BaseTypes.Count; i++)
                {
                    _Writer.Write(i == 0 ? " : " : ", ");
                    declaration.BaseTypes[i].WriteTypeReference(_Writer);
                }
            }

            _Writer.WriteLine(" {");

            if (declaration.HasMember)
            {
                _Writer.Indent++;
                var cv = new InterfaceMemberVisitor(this, false);
                for (var i = 0; i < declaration.Members.Count; i++)
                {
                    declaration.Members[i].Accept(cv);
                    _Writer.WriteLine(';');
                }
                _Writer.Indent--;
            }
            _Writer.Write('}');

            return 0;
        }

        // 8.1
        int IRootStatementVisitor<int>.VisitClassDeclaration(ClassDeclaration declaration)
        {
            WriteClassDeclaration(declaration);
            _Writer.WriteLine();

            return 0;
        }

        int IModuleMemberVisitor<int>.VisitClassDeclaration(ClassDeclaration declaration)
        {
            WriteClassDeclaration(declaration);
            _Writer.WriteLine();

            return 0;
        }

        int INamespaceMemberVisitor<int>.VisitClassDeclaration(ClassDeclaration declaration)
        {
            WriteClassDeclaration(declaration);
            _Writer.WriteLine();

            return 0;
        }

        private int WriteClassDeclaration(ClassDeclaration declaration)
        {
            if (declaration.HasDecorator)
            {
                WriteDecorators(declaration.Decorators, false);
            }
            WriteIsDeclare(declaration.IsDeclare);
            WriteIsExport(declaration.IsExport);
            WriteIsDefault(declaration.IsDefault);
            WriteIsAbstract(declaration.IsAbstract);
            _Writer.Write("class ");
            _Writer.Write(declaration.Name);

            if (declaration.HasTypeParameter)
            {
                _Writer.WriteTypeParameters(declaration.TypeParameters);
            }

            if (declaration.BaseType != null)
            {
                _Writer.Write(" extends ");
                declaration.BaseType.WriteTypeReference(_Writer);
            }

            if (declaration.HasInterface)
            {
                for (int i = 0; i < declaration.Interface.Count; i++)
                {
                    _Writer.Write(i == 0 ? " implements " : ", ");
                    declaration.Interface[i].WriteTypeReference(_Writer);
                }
            }

            _Writer.WriteLine(" {");

            if (declaration.HasMember)
            {
                _Writer.Indent++;
                var cv = new ClassMemberVisitor(this, declaration.IsDeclare);
                for (var i = 0; i < declaration.Members.Count; i++)
                {
                    declaration.Members[i].Accept(cv);
                }
                _Writer.Indent--;
            }
            _Writer.Write('}');

            return 0;
        }

        // 9.1
        int IRootStatementVisitor<int>.VisitEnumDeclaration(EnumDeclaration declaration)
        {
            WriteEnumDeclaration(declaration);
            _Writer.WriteLine();

            return 0;
        }

        int IModuleMemberVisitor<int>.VisitEnumDeclaration(EnumDeclaration declaration)
        {
            WriteEnumDeclaration(declaration);
            _Writer.WriteLine();

            return 0;
        }

        int INamespaceMemberVisitor<int>.VisitEnumDeclaration(EnumDeclaration declaration)
        {
            WriteEnumDeclaration(declaration);
            _Writer.WriteLine();

            return 0;
        }

        private int WriteEnumDeclaration(EnumDeclaration declaration)
        {
            if (declaration.HasDecorator)
            {
                WriteDecorators(declaration.Decorators, false);
            }
            WriteIsDeclare(declaration.IsDeclare);
            WriteIsExport(declaration.IsExport);
            if (declaration.IsConst)
            {
                _Writer.Write("const ");
            }
            _Writer.Write("enum ");
            _Writer.Write(declaration.Name);
            _Writer.WriteLine(" {");
            if (declaration.HasMember)
            {
                _Writer.Indent++;
                for (var i = 0; i < declaration.Members.Count; i++)
                {
                    var m = declaration.Members[i];
                    _Writer.Write(m.FieldName);
                    if (m.Initializer != null)
                    {
                        _Writer.Write(" = ");
                        m.Initializer.Accept(this);
                    }
                    if (i < declaration.Members.Count - 1)
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

        #endregion Module Member

        #region Member

        // TODO: 6.4 Destructuring Parameter Declarations

        // TODO: 6.7 Generator Functions

        // TODO: 6.8 Asynchronous Functions

        // TODO: 6.9 Type Guard Functions

        #region WriteField

        private void WriteField(FieldDeclaration member)
        {
            if (member.HasDecorator)
            {
                WriteDecorators(member.Decorators, false);
            }
            _Writer.WriteAccessibility(member.Accessibility);
            WriteIsStatic(member.IsStatic);
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
            _Writer.Write("constructor ");
            WriteCallSignature(signature ?? member);
        }

        #endregion WriteConstructor

        #region WriteGetAccessor/WriteSetAccessor

        private void WriteGetAccessor(AccessorDeclaration member, bool skipBody)
        {
            if (member.HasDecorator)
            {
                WriteDecorators(member.Decorators, false);
            }
            _Writer.WriteAccessibility(member.Accessibility);
            WriteIsStatic(member.IsStatic);
            _Writer.Write("get ");
            _Writer.Write(member.PropertyName);
            _Writer.Write("()");
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
            if (member.HasDecorator)
            {
                WriteDecorators(member.Decorators, false);
            }
            _Writer.WriteAccessibility(member.Accessibility);
            WriteIsStatic(member.IsStatic);
            _Writer.Write("set ");
            _Writer.Write(member.PropertyName);
            _Writer.Write("(");
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
            if (member.HasDecorator)
            {
                WriteDecorators(member.Decorators, false);
            }
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
            WriteIsStatic(member.IsStatic);
            WriteIsAbstract(member.IsAbstract);
            _Writer.Write(member.MethodName);
            WriteCallSignature(signature ?? member);
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

        private void WriteDecorators(Collection<Decorator> decorators, bool inline)
        {
            foreach (var d in decorators)
            {
                _Writer.Write('@');
                _Writer.Write(d.Name);
                if (d.HasParameter)
                {
                    _Writer.WriteParameters(d.Parameters, this);
                }
                if (inline)
                {
                    _Writer.Write(' ');
                }
                else
                {
                    _Writer.WriteLine();
                }
            }
        }

        private void WriteCallSignature(ICallSignature signature)
        {
            _Writer.WriteTypeParameters(signature.TypeParameters);
            if (signature.HasParameter)
            {
                WriteParameters(signature.Parameters);
            }
            else
            {
                _Writer.Write("()");
            }
            if (signature.ReturnType != null)
            {
                _Writer.Write(": ");
                signature.ReturnType.WriteTypeReference(_Writer);
            }
        }

        internal void WriteParameters(Collection<Parameter> parameters)
        {
            _Writer.Write('(');

            if (parameters?.Count > 0)
            {
                for (var i = 0; i < parameters.Count; i++)
                {
                    if (i > 0)
                    {
                        _Writer.Write(", ");
                    }

                    var tp = parameters[i];
                    if (tp.IsRest)
                    {
                        _Writer.Write("...");
                    }

                    if (tp.HasDecorator)
                    {
                        WriteDecorators(tp.Decorators, true);
                    }

                    _Writer.WriteAccessibility(tp.Accessibility);
                    _Writer.Write(tp.ParameterName);
                    if (tp.IsOptional && tp.Initializer != null)
                    {
                        _Writer.Write('?');
                    }

                    if (tp.ParameterType != null)
                    {
                        _Writer.Write(": ");
                        tp.ParameterType.WriteTypeReference(_Writer);
                    }

                    if (tp.Initializer != null)
                    {
                        _Writer.Write(" = ");

                        tp.Initializer.Accept(this);
                    }
                }
            }
            _Writer.Write(')');
        }

        #endregion Declarations

        int IRootStatementVisitor<int>.VisitModuleDeclaration(ModuleDeclaration module)
        {
            WriteIsDeclare(module.IsDeclare);
            WriteIsExport(module.IsExport);
            _Writer.Write("module ");
            _Writer.Write(module.Name);
            _Writer.WriteLine(" {");
            if (module.HasMember)
            {
                _Writer.Indent++;
                foreach (var m in module.Members)
                {
                    m.Accept(this);
                }
                _Writer.Indent--;
            }
            _Writer.WriteLine('}');

            return 0;
        }

        int IRootStatementVisitor<int>.VisitNamespaceDeclaration(NamespaceDeclaration module)
        {
            WriteIsDeclare(module.IsDeclare);
            WriteIsExport(module.IsExport);
            _Writer.Write("module ");
            _Writer.Write(module.Name);
            _Writer.WriteLine(" {");
            if (module.HasMember)
            {
                _Writer.Indent++;
                foreach (var m in module.Members)
                {
                    m.Accept(this);
                }
                _Writer.Indent--;
            }
            _Writer.WriteLine('}');

            return 0;
        }
    }
}
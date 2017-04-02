using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipwreck.TypeScriptModels.Statements;

namespace Shipwreck.TypeScriptModels
{
    public interface IStatementVistor<T>
    {
        // 5.2
        T VisitVariableDeclaration(VariableDeclaration statement);

        // 5.3
        T VisitLetDeclaration(LetDeclaration statement);

        T VisitConstDeclaration(ConstDeclaration statement);

        // 5.4
        T VisitIf(IfStatement expression);

        // 5.4
        T VisitDo(DoStatement expression);

        // 5.4
        T VisitWhile(WhileStatement expression);

        // 5.5
        T VisitFor(ForStatement expression);

        // 5.6
        T VisitForIn(ForInStatement expression);

        // 5.7
        T VisitForOf(ForOfStatement expression);

        // 5.8
        T VisitContinue(ContinueStatement expression);

        // 5.9
        T VisitBreak(BreakStatement expression);

        // 5.10
        T VisitReturn(ReturnStatement expression);

        // 5.11
        T VisitWith(WithStatement expression);

        // 5.12
        T VisitSwith(SwitchStatement expression);

        // 5.13
        T VisitThrow(ThrowStatement expression);

        // 5.14
        T VisitTry(TryStatement expression);
    }
}
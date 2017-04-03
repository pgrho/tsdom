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
        // 5.3
        T VisitVariableDeclaration(VariableDeclaration statement);

        // 5.4
        T VisitIf(IfStatement statement);

        // 5.4
        T VisitDo(DoStatement statement);

        // 5.4
        T VisitWhile(WhileStatement statement);

        // 5.5
        T VisitFor(ForStatement statement);

        // 5.6
        T VisitForIn(ForInStatement statement);

        // 5.7
        T VisitForOf(ForOfStatement statement);

        // 5.8
        T VisitContinue(ContinueStatement statement);

        // 5.9
        T VisitBreak(BreakStatement statement);

        // 5.10
        T VisitReturn(ReturnStatement statement);

        // 5.11
        T VisitWith(WithStatement statement);

        // 5.12
        T VisitSwitch(SwitchStatement statement);

        // 5.13
        T VisitThrow(ThrowStatement statement);

        // 5.14
        T VisitTry(TryStatement statement);
    }
}
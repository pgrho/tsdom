using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipwreck.TypeScriptModels.Declarations;

namespace Shipwreck.TypeScriptModels.Expressions
{
    public interface IObjectLiteralVisitor<T>
    {
        T VisitMemberInitializer(ObjectMemberInitializer member);

        T VisitMethod(MethodDeclaration member);

        T VisitGetAccessor(GetAccessorDeclaration member);
        T VisitSetAccessor(SetAccessorDeclaration member);
    }
}

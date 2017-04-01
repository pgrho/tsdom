using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Shipwreck.TypeScriptModels
{
    public abstract class MemberDeclaration : Declaration
    {
        public AccessModifier AccessModifier { get; set; }

        public bool IsStatic { get; set; }

        public TypeDeclaration DeclaringType
            => Owner as TypeDeclaration;
    }
}
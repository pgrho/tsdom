﻿using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipwreck.TypeScriptModels.Expressions;

namespace Shipwreck.TypeScriptModels.Declarations
{
    // 8.4.2
    public sealed class MethodDeclaration : FunctionDeclarationBase, IClassMember, IInterfaceMember, ICallSignature
    {
        /// <summary>
        /// Gets or sets the value representing the accessibility modifier of the method.
        /// </summary>
        [DefaultValue(AccessibilityModifier.None)]
        public AccessibilityModifier Accessibility { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether the method has a <c>static</c> modifier.
        /// </summary>
        [DefaultValue(false)]
        public bool IsStatic { get; set; }

        /// <summary>
        /// Gets or sets the binding identifier of this method.
        /// </summary>
        [DefaultValue(null)]
        public string MethodName { get; set; }

        public void Accept<T>(IClassMemberVisitor<T> visitor)
            => visitor.VisitMethod(this);

        public void Accept<T>(IInterfaceMemberVisitor<T> visitor)
            => visitor.VisitMethod(this);
    }
}
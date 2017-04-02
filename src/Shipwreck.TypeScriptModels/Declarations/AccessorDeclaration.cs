﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shipwreck.TypeScriptModels.Expressions;

namespace Shipwreck.TypeScriptModels.Declarations
{
    public abstract class AccessorDeclaration : IObjectLiteralMember
    {
        public AccessibilityModifier Accessibility { get; set; }
        public string PropertyName { get; set; }
        public ITypeReference PropertyType { get; set; }

        public bool IsSet => GetIsSet();

        internal abstract bool GetIsSet();

        private Collection<Statement> _Statements;

        public bool HasStatement
            => _Statements?.Count > 0;

        public Collection<Statement> Statements
        {
            get
            {
                return CollectionHelper.GetOrCreate(ref _Statements);
            }
            set
            {
                CollectionHelper.Set(ref _Statements, value);
            }
        }

        public abstract void Accept<T>(IObjectLiteralVisitor<T> visitor);
    }
}
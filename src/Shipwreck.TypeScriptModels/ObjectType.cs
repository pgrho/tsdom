using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Shipwreck.TypeScriptModels
{
    public class ObjectType : ITypeScriptType
    {
        private List<ObjectTypeProperty> _Properties;

        public string Name
        {
            get
            {
                // TODO:
                return "Object";
            }
        }

        public List<ObjectTypeProperty> Properties
        {
            get
            {
                return _Properties ?? (_Properties = new List<ObjectTypeProperty>());
            }
            set
            {
                _Properties?.Clear();
                if (value?.Count > 0)
                {
                    Properties.AddRange(value);
                }
            }
        }

        public void WriteTypeName(TextWriter writer)
        {
            if (_Properties?.Count > 0)
            {
                writer.Write("{ ");

                for (var i = 0; i < _Properties.Count; i++)
                {
                    if (i > 0)
                    {
                        writer.Write(", ");
                    }
                    var pt = _Properties[i];
                    writer.Write(pt.Name);
                    if (!pt.IsRequired)
                    {
                        writer.Write('?');
                    }
                    writer.Write(": ");
                    pt.PropertyType.WriteTypeName(writer);
                }

                writer.Write(" }");
            }
            else
            {
                writer.Write("{}");
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;

namespace Shipwreck.TypeScriptModels
{
    /// <summary>
    /// Provides the base class from which the classes that represent syntax tree nodes are derived.
    /// </summary>
    public abstract class Syntax
    {
        internal Syntax()
        {
        }

        private Dictionary<object, object> _Annotations;

        public object GetAnnotation(object key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            object r = null;
            _Annotations?.TryGetValue(key, out r);
            return r;
        }

        public void SetAnnotation(object key, object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (value == null)
            {
                _Annotations?.Remove(key);
            }
            else
            {
                (_Annotations ?? (_Annotations = new Dictionary<object, object>()))[key] = value;
            }
        }

        public override string ToString()
        {
            using (var w = new StringWriter())
            using (var tsw = new TypeScriptWriter(w))
            {
                tsw.Write(this);
                tsw.Flush();

                return w.ToString();
            }
        }
    }
}
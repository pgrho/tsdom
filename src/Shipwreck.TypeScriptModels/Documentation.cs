using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Shipwreck.TypeScriptModels
{
    public class Documentation
    {
        private List<ParameterDocumentation> _Parameters;

        public string Summary { get; set; }
        public string Returns { get; set; } 

        public List<ParameterDocumentation> Parameters
        {
            get
            {
                return _Parameters ?? (_Parameters = new List<ParameterDocumentation>());
            }
            set
            {
                _Parameters?.Clear();
                if (value?.Count > 0)
                {
                    Parameters.AddRange(value);
                }
            }
        }
    }
}
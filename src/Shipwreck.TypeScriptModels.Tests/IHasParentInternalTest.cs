using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shipwreck.TypeScriptModels
{
    [TestClass]
    public class IHasParentInternalTest
    {
        [TestMethod]
        public void InspectIHasParent()
        {
            var a = typeof(IHasParent).Assembly;
            var failed = a.GetTypes().Where(t => t.IsClass && typeof(IHasParent).IsAssignableFrom(t) && !typeof(IHasParentInternal).IsAssignableFrom(t)).ToArray();

            Assert.IsFalse(failed.Any(), $"Following classes must implement {nameof(IHasParentInternal)}: {string.Join(",", failed.Select(t => t.FullName))}");
        }
    }
}

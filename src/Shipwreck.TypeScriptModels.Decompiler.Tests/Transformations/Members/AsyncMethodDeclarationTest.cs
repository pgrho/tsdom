using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Members
{
    [TestClass]
    public class AsyncMethodDeclarationTest
    {
        private  class TestClass
        {
            private async Task Method1()
            {
                await Task.Delay(150);
                await Task.Delay(250);
                await Task.Delay(350);
            }
        }

        [TestMethod]
#if DEBUG
        [ExpectedException(typeof(NotSupportedException))]
#endif
        public void AsyncMethodDeclaration_Test()
        {
            var f = new TypeTranslationContext<TestClass>().GetMethod("Method1");
            Assert.IsNotNull(f);
        }
    }
}

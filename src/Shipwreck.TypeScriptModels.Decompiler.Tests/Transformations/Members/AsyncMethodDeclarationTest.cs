using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Members
{
    public class AsyncMethodDeclarationTestClass
    {
        private async Task Method1()
        {
            await Task.Delay(150);
            await Task.Delay(250);
            await Task.Delay(350);
        }
    }

    [TestClass]
    public class AsyncMethodDeclarationTest : TransformationTestBase<AsyncMethodDeclarationTestClass>
    {
        [TestMethod]
        public void AsyncMethodDeclaration_TransformTest()
        {
            var f = GetMethod("Method1");

            Assert.IsNotNull(f);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Statements
{
    public class UsingStatementTestClass
    {
        public void SourceMethod(IDisposable d)
        {
            using (var a = Create())
            {
                a.ReadByte();
            }
            //using (d)
            //{

            //}
        }

        private MemoryStream Create() => null;
    }

    [TestClass]
    public class UsingStatementTest : TransformationTestBase<UsingStatementTestClass>
    {
        [TestMethod]
        public void UsingStatement_TransformTest()
        {
            var m = GetMethod(nameof(UsingStatementTestClass.SourceMethod));

            Assert.IsNotNull(m);
        }
    }
}

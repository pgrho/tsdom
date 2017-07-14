using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Statements
{
    [TestClass]
    public class UsingStatementTest
    {
        private class TestClass
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

        [TestMethod]
        public void UsingStatement_TransformTest()
        {
            var t = new TransformingContext<TestClass>();
            var m = t.GetMethod(nameof(TestClass.SourceMethod));

            Assert.IsNotNull(m);
        }
    }
}
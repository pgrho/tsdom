﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Statements
{
    [TestClass]
    public class ForOfStatementTest
    {
        private class TestClass
        {
            public void SourceMethod(IEnumerable a)
            {
                foreach (var e in a)
                {
                    e.ToString();
                }
            }
        }

        [TestMethod]
        public void ForOfStatement_TransformTest()
        {
            var t = new TransformingContext<TestClass>();
            var m = t.GetMethod(nameof(TestClass.SourceMethod));

            Assert.IsNotNull(m);
        }
    }
}
﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Statements
{
    [TestClass]
    public class SwitchStatementTest
    {
        private class TestClass
        {
            public string SourceMethod(int a)
            {
                switch (a)
                {
                    case 0:
                        return "a";

                    case 1:
                        return "b";

                    case 2:
                        return "c";

                    case 3:
                        return "d";

                    case 4:
                        return "e";

                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                        return "fghij";

                    default:
                        return "other";
                }
            }
        }

        [TestMethod]
        public void SwitchStatement_Test()
        {
            var t = new TypeTranslationContext<TestClass>();
            var m = t.GetMethod(nameof(TestClass.SourceMethod));

            Assert.IsNotNull(m);
        }
    }
}
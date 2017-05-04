using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Statements
{
    public class SwitchStatementTestClass
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

    [TestClass]
    public class SwitchStatementTest : TransformationTestBase<SwitchStatementTestClass>
    {
        [TestMethod]
        public void SwitchStatement_TransformTest()
        {
            var m = GetMethod(nameof(SwitchStatementTestClass.SourceMethod));

            Assert.IsNotNull(m);
        }
    }
}

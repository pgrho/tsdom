using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Members
{
    internal class EventDeclarationTestClass
    {
        public event EventHandler TestEvent;

        public void OnTestEvent()
            => TestEvent?.Invoke(this, EventArgs.Empty);
    }

    [TestClass]
    public class EventDeclarationTest
    {
        [TestMethod]
        public void EventDeclaration_TransformTest()
        {
            var t = new TransformationTestBase<EventDeclarationTestClass>();
            Assert.IsNotNull(t.GetField("__TestEvent"));
            Assert.IsNotNull(t.GetMethod("add_TestEvent"));
            Assert.IsNotNull(t.GetMethod("remove_TestEvent"));
        }
    }
}
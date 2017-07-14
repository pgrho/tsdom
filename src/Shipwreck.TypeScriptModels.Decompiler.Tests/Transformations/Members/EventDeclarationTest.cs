using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Shipwreck.TypeScriptModels.Decompiler.Transformations.Members
{
    [TestClass]
    public class EventDeclarationTest
    {
        private class TestClass
        {
            public event EventHandler TestEvent;

            public void OnTestEvent()
                => TestEvent?.Invoke(this, EventArgs.Empty);
        }

        [TestMethod]
        public void EventDeclaration_TransformTest()
        {
            var t = new TransformingContext<TestClass>();
            Assert.IsNotNull(t.GetField("__TestEvent"));
            Assert.IsNotNull(t.GetMethod("add_TestEvent"));
            Assert.IsNotNull(t.GetMethod("remove_TestEvent"));
        }
    }
}
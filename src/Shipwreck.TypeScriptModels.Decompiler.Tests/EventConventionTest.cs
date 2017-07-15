using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Shipwreck.TypeScriptModels.Decompiler
{
    [TestClass]
    public class EventConventionTest
    {
        private class Auto
        {
            public event EventHandler TestEvent;

            public void OnTestEvent()
                => TestEvent?.Invoke(this, EventArgs.Empty);
        }

        private class Custom
        {
            public event EventHandler TestEvent
            {
                add { }
                remove { }
            }
        }

        [TestMethod]
        public void EventDeclaration_TransformTest_Auto()
        {
            var t = new TranslatingContext<Auto>();
            Assert.IsNotNull(t.GetField("$ev_TestEvent"));
            Assert.IsNotNull(t.GetMethod("$addev_TestEvent"));
            Assert.IsNotNull(t.GetMethod("$remev_TestEvent"));
        }

        [TestMethod]
        public void EventDeclaration_TransformTest_Custom()
        {
            var t = new TranslatingContext<Custom>();
            Assert.IsNull(t.GetField("$ev_TestEvent"));
            Assert.IsNotNull(t.GetMethod("$addev_TestEvent"));
            Assert.IsNotNull(t.GetMethod("$remev_TestEvent"));
        }
    }
}
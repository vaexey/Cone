using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Model.Entry.Bind
{
    [TestClass]
    public class BindTriggerTest
    {
        [TestMethod]
        public void OnPoll()
        {
            var bt = new BindTrigger();

            Assert.IsTrue(bt.Validate(1, true));
            Assert.IsFalse(bt.Validate(1, false));
        }

        [TestMethod]
        public void OnValue()
        {
            var bt = new BindTrigger();

            bt.Minimum = 4;
            bt.Maximum = 5;
            bt.ValueMatch = true;
            bt.PollMatch = false;

            Assert.IsTrue(bt.Validate(4));
            Assert.IsTrue(bt.Validate(5));

            Assert.IsFalse(bt.Validate(3.9));
            Assert.IsFalse(bt.Validate(5.1));
            Assert.IsTrue(bt.Validate(4.1));

            Assert.IsFalse(bt.Validate(3.9, true));
        }
    }
}

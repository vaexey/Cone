using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Model.Entry.Bind
{
    [TestClass]
    public class BindEntryTest
    {
        public class TestBindNode : BindNode
        {
            public double LastValue { get; set; }
            public TestBindNode(double firstValue = 0)
            {
                LastValue = firstValue;
            }

            public override double Get()
            {
                return LastValue;
            }

            public override void Set(double value)
            {
                LastValue = value;
            }

            public void SetChange(double value)
            {
                Set(value);
                SetPoll(true);
            }
        }

        private BindEntry createBindEntry(int inputs = 1, int outputs = 1)
        {
            var be = new BindEntry();

            for(int i = 0; i < inputs; i++)
                be.Inputs.Add(new TestBindNode(0));

            for(int i = 0; i < outputs; i++)
                be.Outputs.Add(new TestBindNode(0));

            return be;
        }

        [TestMethod]
        public void InputUpdate_1I()
        {
            var be = createBindEntry();

            be.Update(null);
            Assert.IsFalse(be.LastSnapshot.ActivatedInputs.Any());

            be.MainInput.Set(1);

            be.Update(null);
            Assert.IsFalse(be.LastSnapshot.ActivatedInputs.Any());

            be.MainInput.SetPoll(true);

            be.Update(null);
            Assert.IsTrue(be.LastSnapshot.ActivatedInputs.Any());
        }

        [TestMethod]
        public void InputUpdate_2I()
        {
            var be = createBindEntry(inputs: 2);

            be.Inputs[0].Set(1);
            be.Inputs[1].Set(1);

            be.Inputs[0].SetPoll(true);

            be.Update(null);
            Assert.AreEqual(1, be.LastSnapshot.ActivatedInputs.Length);

            be.Inputs[0].SetPoll(true);
            be.Inputs[1].SetPoll(true);

            be.Update(null);
            Assert.AreEqual(2, be.LastSnapshot.ActivatedInputs.Length);

            be.Update(null);
            Assert.AreEqual(0, be.LastSnapshot.ActivatedInputs.Length);
        }

        [TestMethod]
        public void ScalingValues()
        {
            var be = createBindEntry(inputs: 3, outputs: 2);

            be.Inputs[0].Set(1);
            be.Inputs[1].Set(2);
            // Sum: 3

            be.Inputs[2].Set(3);
            // No poll

            be.InputScaling.FromMin = 2;
            be.InputScaling.FromMax = 7;
            // 0-1: 0.2

            be.InputScaling.ToMin = 10;
            be.InputScaling.ToMax = 20;
            // 10-20: 12

            be.Trigger.ValueMatch = true;
            be.Trigger.Minimum = 12;
            be.Trigger.Maximum = 12;

            be.OutputScaling.FromMin = 10;
            be.OutputScaling.FromMax = 14;
            // 0-1: 0.5

            be.OutputScaling.ToMin = 0;
            be.OutputScaling.ToMax = 1;
            // 0-1: 0.5

            be.Method.Out = BindMethod.SimpleModeOut.SPLIT;
            // 0.25

            be.Inputs[0].SetPoll(true);
            be.Inputs[1].SetPoll(true);
            be.Update(null);

            var snap = be.LastSnapshot;

            Assert.AreEqual(1, snap.Combined[0]);
            Assert.AreEqual(2, snap.Combined[1]);

            Assert.AreEqual(3, snap.ValueCombined);
            Assert.AreEqual(12, snap.ValueInputScaled);
            Assert.AreEqual(0.5, snap.ValueOutputScaled);

            Assert.AreEqual(0.25, snap.Distribution[0]);

            Assert.IsTrue(snap.Triggered);
        }

        [TestMethod]
        public void OutputDistribution()
        {
            var be = createBindEntry(1, 2);

            be.MainOutput.Set(1);
            be.MainOutput.SetPoll(true);

            be.Update(null);

            Assert.AreEqual(1, be.MainOutput.Get());
        }
    }
}

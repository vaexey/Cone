using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Model.Entry.Bind
{
    [TestClass]
    public class BindMethodTest
    {
        [TestMethod]
        public void CombineSum()
        {
            var method = new BindMethod();

            method.In = BindMethod.SimpleModeIn.SUM;

            Assert.AreEqual(
                10.4,
                method.Combine(new double[] { 1,1, 2,5,1.4}, 5)
                );
        }

        [TestMethod]
        public void CombineAverage()
        {
            var method = new BindMethod();

            method.In = BindMethod.SimpleModeIn.AVERAGE;

            Assert.AreEqual(
                2.08,
                method.Combine(new double[] { 1, 1, 2, 5, 1.4 }, 5)
                );
        }

        [TestMethod]
        public void CombineFirst()
        {
            var method = new BindMethod();

            method.In = BindMethod.SimpleModeIn.FIRST;

            Assert.AreEqual(
                1.4,
                method.Combine(new double[] { 1.4, 1, 2, 5, 1 }, 5)
                );
        }

        [TestMethod]
        public void DistributeRepeat()
        {
            var method = new BindMethod();

            method.Out = BindMethod.SimpleModeOut.REPEAT;

            var dist = method.Distribute(5.4, 5);

            foreach (var d in dist)
                Assert.AreEqual(5.4, d);
        }

        [TestMethod]
        public void DistributeSplit()
        {
            var method = new BindMethod();

            method.Out = BindMethod.SimpleModeOut.SPLIT;

            var dist = method.Distribute(5.4, 5);

            foreach (var d in dist)
                Assert.AreEqual(5.4 / 5, d);
        }
    }
}

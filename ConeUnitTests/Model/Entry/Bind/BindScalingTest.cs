using Newtonsoft.Json.Linq;

namespace ConeEngine.Model.Entry.Bind
{
    [TestClass]
    public class BindScalingTest
    {
        private JObject generateConfig(int f = 0, int fm = 1, int t = 0, int tm = 1, int modulo = 0, int round = -1)
        {
            return JObject.Parse($@"
{{
    ""from"": [{f}, {fm}],
    ""to"": [{t}, {tm}],
    ""modulo"": {modulo},
    ""round"": {round}
}}
            ");
        }

        [TestMethod]
        public void DefaultValues()
        {
            var bs = new BindScaling();

            bs.Deserialize(generateConfig(), null);

            Assert.AreEqual(1, bs.ScaleForward(1));
            Assert.AreEqual(1, bs.ScaleBackward(1));
        }

        [TestMethod]
        public void Modulo()
        {
            var bs3 = new BindScaling();
            var bs5 = new BindScaling();

            bs3.Deserialize(generateConfig(-10, 10, 1, 2,modulo: 3), null);
            bs5.Deserialize(generateConfig(-10, 10, 3, 5,modulo: 5), null);

            Assert.AreEqual(1.5, bs3.ScaleForward(6));
            Assert.AreEqual(4.1, bs5.ScaleForward(6));
        }
    }
}

using ConeEngine.Model.Entry.Bind;
using ConeEngine.Model.Flow;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Internal.BindNodes
{
    public class RNGInternalBindNode : BindNode
    {
        protected Random rnd = new();

        public double Minimum { get; set; } = 0;
        public double Maximum { get; set; } = 1;

        public override double Get()
        {
            return rnd.NextDouble() * (Maximum - Minimum) + Minimum;
        }

        public override void Set(double value)
        {
            throw new Exception("Cannot set random generator value.");
        }

        public override bool HasPoll(bool reset = false)
        {
            return true;
        }

        public override void Deserialize(JObject config, Context ctx)
        {
            base.Deserialize(config, ctx);

            Minimum = config.Value<double>("min");
            Maximum = config.Value<double>("max");
        }
    }
}

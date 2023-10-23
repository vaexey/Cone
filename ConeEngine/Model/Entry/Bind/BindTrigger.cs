using ConeEngine.Model.Flow;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Model.Entry.Bind
{
    public class BindTrigger
    {
        public virtual double Minimum { get; set; } = 0;
        public virtual double Maximum { get; set; } = 1;
        public bool ValueMatch { get; set; } = false;
        public bool PollMatch { get; set; } = true;

        public virtual bool Validate(double value, bool poll = false)
        {
            return (!PollMatch || poll) &&
                (!ValueMatch || (CmpDbl(Minimum, value) && CmpDbl(value, Maximum)));
        }

        protected bool CmpDbl(double smaller, double bigger)
        {
            return bigger >= smaller || Math.Abs(bigger - smaller) < 0.01;
        }

        public virtual void Deserialize(JObject config, Context ctx)
        {
            Minimum = config.Value<double>("min");
            Maximum = config.Value<double>("max");

            ValueMatch = config.Value<bool>("value");
            PollMatch = config.Value<bool>("poll");
        }
    }
}

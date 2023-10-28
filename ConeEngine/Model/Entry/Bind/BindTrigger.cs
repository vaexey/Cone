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
        public bool ChangeMatch { get; set; } = false;

        public double LastChangeValue { get; set; } = double.MaxValue;

        public virtual bool Validate(double value, bool poll = false)
        {
            var change = ValidateChange(value);

            return (!PollMatch || poll) &&
                   (!ChangeMatch || change) &&
                   (!ValueMatch || (CmpDbl(Minimum, value) && CmpDbl(value, Maximum)));
        }

        protected bool EqDbl(double v1, double v2)
        {
            return Math.Abs(v1 - v2) < 0.01;
        }

        protected bool CmpDbl(double smaller, double bigger)
        {
            return bigger >= smaller || EqDbl(bigger, smaller);
        }

        protected bool ValidateChange(double newValue)
        {
            var change = !EqDbl(newValue, LastChangeValue);

            if(change)
                LastChangeValue = newValue;

            return change;
        }

        public virtual void Deserialize(JObject config, Context ctx)
        {
            Minimum = config.Value<double>("min");
            Maximum = config.Value<double>("max");

            ValueMatch = config.Value<bool>("value");
            PollMatch = config.Value<bool>("poll");
            ChangeMatch = config.Value<bool>("change");
        }
    }
}

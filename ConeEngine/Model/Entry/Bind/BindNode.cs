using ConeEngine.Model.Flow;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Model.Entry.Bind
{
    public abstract class BindNode
    {
        public BindScaling Prescaler { get; set; } = new();
        public BindScaling Scaling { get; set; } = new();

        public BindScaling TriggerScaling { get; set; } = new();
        public BindTrigger Trigger { get; set; } = new();

        public abstract double Get();
        public abstract void Set(double value);

        protected bool hasPoll = false;
        public virtual bool HasPoll(bool reset = false)
        {
            var r = hasPoll;

            if(reset)
                hasPoll = false;

            return r;
        }

        public void SetPoll(bool ch)
        {
            hasPoll = ch;
        }

        public virtual void Deserialize(JObject config, Context ctx)
        {
            if (config["scale"] is JObject scalej)
                Scaling.Deserialize(scalej, ctx);

            if (config["prescaler"] is JObject pscalej)
                Prescaler.Deserialize(pscalej, ctx);

            if (config["trigger"] is JObject triggerj)
            {
                Trigger.Deserialize(triggerj, ctx);
                TriggerScaling.Deserialize(triggerj, ctx);
            }
        }
    }
}

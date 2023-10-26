using ConeEngine.Model.Entry.Bind;
using ConeEngine.Model.Flow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Model.Entry.Event
{
    public class BindEventNode : EventNode
    {
        public BindNode Target { get; set; }

        public BindEventNode(BindNode target)
        {
            Target = target;
        }

        public override bool Poll(Context ctx)
        {
            //return Target.Trigger.Validate(
            //    Target.Get()) && (!OnChange || Target.Diff());
            //return false;
            var poll = Target.HasPoll(true);
            
            var value = Target.Get();
            var prescaled = Target.Prescaler.ScaleForward(value);
            var scaled = Target.Scaling.ScaleForward(prescaled);

            var valid = Target.Trigger.Validate(
                    scaled, poll
                );

            return valid;
        }
    }
}

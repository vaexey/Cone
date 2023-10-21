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
        public bool OnChange = false;

        public BindEventNode(BindNode target)
        {
            Target = target;
        }

        public override bool Poll(Context ctx)
        {
            return Target.Trigger.Validate(
                Target.Get()) && (!OnChange || Target.Diff());
        }
    }
}

using ConeEngine.Model.Entry;
using ConeEngine.Model.Entry.Action;
using ConeEngine.Model.Entry.Bind;
using ConeEngine.Model.Flow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Internal.Actions
{
    public class ToggleConeInternalAction : CAction
    {
        public string NodeID { get; set; }
        public double Minimum { get; set; }
        public double Maximum { get; set; }

        public ToggleConeInternalAction(string node, double minimum, double maximum)
        {
            NodeID = node;
            Minimum = minimum;
            Maximum = maximum;
        }

        public override Result Execute(Context ctx, object[] args)
        {
            var node = ctx.GetEntry<BindEntry>(NodeID);

            var current = node.MainOutput.Get();

            if(current < Minimum || current > Maximum)
            {
                node.MainOutput.Set(Minimum);
            }
            else if(current > Minimum)
            {
                node.MainOutput.Set(Minimum);
            }
            else
            {
                node.MainOutput.Set(Maximum);
            }

            return Result.OK;
        }
    }
}

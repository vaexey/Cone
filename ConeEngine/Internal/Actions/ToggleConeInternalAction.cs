using ConeEngine.Model.Entry;
using ConeEngine.Model.Entry.Action;
using ConeEngine.Model.Entry.Bind;
using ConeEngine.Model.Flow;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Internal.Actions
{
    public class ToggleConeInternalAction : CAction
    {
        public string NodeID { get; set; } = "";
        public double Minimum { get; set; } = 0;
        public double Maximum { get; set; } = 1;
        
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

        public override void Deserialize(JObject config, Context ctx)
        {
            base.Deserialize(config, ctx);

            Minimum = config.Value<double>("min");
            Maximum = config.Value<double>("max");

            if (config.Value<string>("id") is string jid)
                NodeID = jid;
        }
    }
}

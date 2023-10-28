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
    public class SetInternalAction : CAction
    {
        public string ID { get; set; } = "";
        public string Source { get; set; } = "";
        public BindScaling Scale { get; set; } = new();

        public double Value { get; set; } = 0;

        public override Result Execute(Context ctx, object[] args)
        {
            double src;

            src = Value;

            if (Source != "")
            {
                var ent = ctx.GetBindEntry(Source);
                
                if(ent.Inputs.Count > 0)
                {
                    src = ent.MainInput.Get();
                }
                else if(ent.Outputs.Count > 0)
                {
                    src = ent.MainOutput.Get();
                }
            }

            var scaled = Scale.ScaleForward(src);

            ctx.GetBindEntry(ID).MainOutput.Set(scaled);

            return Result.OK;
        }

        public override void Deserialize(JObject config, Context ctx)
        {
            base.Deserialize(config, ctx);

            if (config.ContainsKey("value"))
                Value = config.Value<double>("value");

            if (config.Value<string>("source") is string jsrc)
                Source = jsrc;

            if (config.Value<string>("id") is string id)
                ID = id;

            if (config["scale"] is JObject scalej)
                Scale.Deserialize(scalej, ctx);
        }
    }
}

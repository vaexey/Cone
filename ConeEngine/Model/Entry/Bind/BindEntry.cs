using ConeEngine.Model.Flow;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Model.Entry.Bind
{
    public class BindEntry : Entry
    {
        public override EntryType? Type => EntryType.BIND;

        public virtual List<BindNode> Inputs { get; set; } = new();
        public virtual List<BindNode> Outputs { get; set; } = new();

        public virtual BindNode MainInput { get => Inputs.First(); }
        public virtual BindNode MainOutput { get => Outputs.First(); }

        public virtual BindScaling InputScaling { get; set; } = new BindScaling();
        public virtual BindScaling OutputScaling { get; set; } = new BindScaling();

        public virtual BindTrigger Trigger { get; set; } = new BindTrigger();

        public virtual BindMethod Method { get; set; } = new BindMethod();

        public virtual BindDirection Direction { get; set; } = BindDirection.FORWARD;

        public override Result Update(Context ctx)
        {
            var inputUpdates = GetUpdatesOf(Inputs);

            var doInputUpdate = inputUpdates.Any();

            if (doInputUpdate)
                DoInputUpdate(inputUpdates);

            return Result.OK;
        }

        protected virtual void DoInputUpdate(BindNode[] nodes)
        {
            var combinedValue = Method.Combine(
                nodes.Select(node =>
                    node.Scaling.ScaleForward(
                        node.Prescaler.ScaleForward(
                            node.Get()
                        )
                    )
               ),
                nodes.Length
            );

            var inputScaled = InputScaling.ScaleForward(combinedValue);

            var triggerStatus = Trigger.Validate(inputScaled, true);

            var outputScaled = OutputScaling.ScaleForward(inputScaled);

            var distributed = Method.Distribute(outputScaled, Outputs.Count);

            foreach(var zip in Outputs.Zip(distributed, (a,b) => new { Output = a, Value = b }))
            {
                var output = zip.Output;
                var value = zip.Value;

                var mainScaled = output.Scaling.ScaleForward(value);
                var triggerScaled = output.TriggerScaling.ScaleForward(mainScaled);
                var prescaled = output.Prescaler.ScaleForward(mainScaled);

                if (output.Trigger.Validate(triggerScaled, true))
                    output.Set(prescaled);
            }
        }

        protected static BindNode[] GetUpdatesOf(List<BindNode> nodes)
        {
            return nodes.Where(node =>
            {
                var poll = node.HasPoll(true);

                var value = node.Get();
                var prescaled = node.Prescaler.ScaleForward(value);
                var tscaled = node.TriggerScaling.ScaleBackward(prescaled);

                return node.Trigger.Validate(value, poll);
            }).ToArray();
        }

        public override void Deserialize(JObject config, Context ctx)
        {
            base.Deserialize(config, ctx);

            var inj = config["input"] as JArray;
            var outj = config["output"] as JArray;

            if (inj == null || outj == null)
                throw new Exception($"Entry ${ID} does not have bind nodes.");

            Inputs = inj.Select(bnj => ctx.InstantiateBindNode(bnj as JObject)).ToList();
            Outputs = outj.Select(bnj => ctx.InstantiateBindNode(bnj as JObject)).ToList();

            if(config.Value<string>("direction") is string direction)
            {
                if (direction == "both")
                    Direction = BindDirection.BOTH;
            }
        }
    }
}

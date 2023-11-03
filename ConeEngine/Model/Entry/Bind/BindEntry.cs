using ConeEngine.Model.Entry.Snapshot;
using ConeEngine.Model.Flow;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ConeEngine.Model.Entry.Bind
{
    public class BindEntry : Entry
    {
        public override EntryType? Type => EntryType.BIND;

        public virtual List<BindNode> Inputs { get; set; } = new();
        public virtual List<BindNode> Outputs { get; set; } = new();

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public virtual BindNode MainInput { get => Inputs.First(); }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public virtual BindNode MainOutput { get => Outputs.First(); }

        public virtual BindScaling InputScaling { get; set; } = new BindScaling();
        public virtual BindScaling OutputScaling { get; set; } = new BindScaling();

        public virtual BindTrigger Trigger { get; set; } = new BindTrigger();

        public virtual BindMethod Method { get; set; } = new BindMethod();

        public virtual BindEntrySnapshot LastSnapshot { get; set; } = new();

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public virtual BindDirection Direction { get; set; } = BindDirection.FORWARD;

        public override Result Update(Context ctx)
        {
            lock(LastSnapshot)
            {
                LastSnapshot.Reset();

                var inputUpdates = GetUpdatesOf(Inputs);

                var doInputUpdate = inputUpdates.Any();

                if (doInputUpdate)
                {
                    DoInputUpdate(inputUpdates);

                    return Result.OK;
                }

                // TODO
                if(Direction == BindDirection.BOTH)
                {
                    var outputUpdates = GetUpdatesOfBackward(Outputs);

                    var doOutputUpdate = outputUpdates.Any();

                    if (doOutputUpdate)
                        DoOutputUpdate(outputUpdates);
                }


            }

            return Result.OK;
        }

        protected virtual void DoInputUpdate(BindNode[] nodes)
        {
            var values = nodes.Select(node =>
                    node.Scaling.ScaleForward(
                        node.Prescaler.ScaleForward(
                            node.Get()
                        )
                    )
               ).ToArray();

            var combinedValue = Method.Combine(
                values,
                nodes.Length
            );

            var inputScaled = InputScaling.ScaleForward(combinedValue);

            var triggerStatus = Trigger.Validate(inputScaled, true);

            var outputScaled = OutputScaling.ScaleForward(inputScaled);

            var distributed = Method.Distribute(outputScaled, Outputs.Count).ToArray();

            if(triggerStatus)
            {
                DistributeOutputValues(distributed);
            }

            LastSnapshot.ActivatedInputs = nodes.Select(n => Inputs.IndexOf(n)).ToArray();
            LastSnapshot.ValueCombined = combinedValue;
            LastSnapshot.ValueInputScaled = inputScaled;
            LastSnapshot.ValueOutputScaled = outputScaled;
            LastSnapshot.Triggered = triggerStatus;
            LastSnapshot.Combined = values;
            LastSnapshot.Distribution = distributed;
        }
        
        protected virtual void DistributeOutputValues(IEnumerable<double> distributed)
        {
            foreach (var zip in Outputs.Zip(distributed, (a, b) => new { Output = a, Value = b }))
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

        // TODO
        protected virtual void DoOutputUpdate(BindNode[] nodes)
        {
            var values = nodes.Select(node =>
                    node.Scaling.ScaleBackward(
                        node.Prescaler.ScaleBackward(
                            node.Get()
                        )
                    )
               ).ToArray();

            var combinedValue = Method.Combine(
                values,
                nodes.Length
            );


            var outputScaled = OutputScaling.ScaleForward(combinedValue);

            var triggerStatus = Trigger.Validate(outputScaled, true);

            var inputScaled = InputScaling.ScaleForward(outputScaled);

            var distributed = Method.Distribute(inputScaled, Outputs.Count).ToArray();

            foreach (var zip in Inputs.Zip(distributed, (a, b) => new { Input = a, Value = b }))
            {
                var input = zip.Input;
                var value = zip.Value;

                var mainScaled = input.Scaling.ScaleBackward(value);
                var triggerScaled = input.TriggerScaling.ScaleBackward(mainScaled);
                var prescaled = input.Prescaler.ScaleBackward(mainScaled);

                if (input.Trigger.Validate(triggerScaled, true))
                    input.Set(prescaled);
            }

            LastSnapshot.ActivatedInputs = nodes.Select(n => Inputs.IndexOf(n)).ToArray();
            LastSnapshot.ValueCombined = combinedValue;
            LastSnapshot.ValueInputScaled = inputScaled;
            LastSnapshot.ValueOutputScaled = outputScaled;
            LastSnapshot.Triggered = triggerStatus;
            LastSnapshot.Combined = values;
            LastSnapshot.Distribution = distributed;
        }

        protected static BindNode[] GetUpdatesOf(List<BindNode> nodes)
        {
            return nodes.Where(node =>
            {
                var poll = node.HasPoll(true);

                var value = node.Get();
                var prescaled = node.Prescaler.ScaleForward(value);
                var tscaled = node.TriggerScaling.ScaleForward(prescaled);

                return node.Trigger.Validate(tscaled, poll);
            }).ToArray();
        }
        
        // TODO
        protected static BindNode[] GetUpdatesOfBackward(List<BindNode> nodes)
        {
            return nodes.Where(node =>
            {
                var poll = node.HasPoll(true);

                var value = node.Get();
                var prescaled = node.Prescaler.ScaleBackward(value);
                var tscaled = node.TriggerScaling.ScaleBackward(prescaled);

                return node.Trigger.Validate(tscaled, poll);
            }).ToArray();
        }

        public override void Deserialize(JObject config, Context ctx)
        {
            base.Deserialize(config, ctx);

            var inj = config["input"] as JArray;
            var outj = config["output"] as JArray;

            if (inj == null || outj == null)
                throw new Exception($"Entry {ID} does not have bind nodes.");

            Inputs = inj.Select(bnj => ctx.InstantiateBindNode(bnj as JObject)).ToList();
            Outputs = outj.Select(bnj => ctx.InstantiateBindNode(bnj as JObject)).ToList();

            if (config["iscale"] is JObject iscalej)
                InputScaling.Deserialize(iscalej, ctx);

            if (config["oscale"] is JObject oscalej)
                OutputScaling.Deserialize(oscalej, ctx);

            if (config["trigger"] is JObject trigj)
                Trigger.Deserialize(trigj, ctx);

            if (config.Value<string>("direction") is string direction)
            {
                if (direction == "both")
                    Direction = BindDirection.BOTH;
            }
        }
    }
}

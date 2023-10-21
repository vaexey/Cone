using ConeEngine.Model.Flow;
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
        public virtual List<BindNode> Inputs { get; set; }
        public virtual List<BindNode> Outputs { get; set; }

        public virtual BindNode MainInput { get => Inputs.First(); }
        public virtual BindNode MainOutput { get => Outputs.First(); }

        public virtual BindMethod Method { get; set; } = new SimpleBindMethod();

        public override EntryType? Type => EntryType.BIND;

        public virtual BindDirection Direction { get; set; } = BindDirection.FORWARD;

        public virtual BindPolicy Policy { get; set; } = BindPolicy.ALWAYS;
        protected double policyLastValue = double.MaxValue;

        public BindEntry()
        {
            Inputs = new();
            Outputs = new();
        }

        public override Result Update(Context ctx)
        {
            var updates = Inputs.Where(i =>
            {
                if (i.Diff())
                    return i.Trigger.Validate(
                            i.TriggerScaling.ScaleForward(
                                /*i.Scaling.ScaleForward(*/
                                    i.Get()
                                /*)*/
                            )
                        );

                return false;
            }).ToArray();

            var doUpdateBack = false;

            if(Direction == BindDirection.BOTH)
            {
                doUpdateBack = Outputs.Select(i =>
                {
                    if (i.Diff())
                        return i.Trigger.Validate(
                                i.TriggerScaling.ScaleForward(
                                    /*i.Scaling.ScaleForward(*/
                                        i.Get()
                                    /*)*/
                                )
                            );

                    return false;
                }).ToArray().Where(u => u).Any();
            }

            //var doUpdate = updates.Where(u => u).Any();
            var doUpdate = updates.Any();

            //if(doUpdate)
            //{

            //    if (Policy == BindPolicy.ON_CHANGE)
            //    {
            //        if (Math.Abs(policyLastValue - input) < 0.01)
            //        {
            //            doUpdate = false;
            //        }
            //    }

            //    policyLastValue = input;
            //}

            if(doUpdate)
            {
                var input = Method.Combine(
                    updates.Select(e => e.Scaling.ScaleForward(e.Get())),
                    updates.Count()
                );

                if (Policy != BindPolicy.ON_CHANGE || Math.Abs(policyLastValue - input) > 0.01)
                {
                    policyLastValue = input;

                    var output = Method.Distribute(
                        input,
                        Outputs.Count
                    );

                    Log.Verbose("Entry {0} trigger & policy passed.", ID);

                    foreach (var o in Outputs.Zip(output, (a, b) => new { Output = a, Value = b }))
                    {
                        o.Output.Set(o.Output.Scaling.ScaleForward(o.Value));
                    }

                    return Result.OK;
                }
            }

            if (doUpdateBack)
            {
                var input = Method.Combine(
                    Outputs.Select(e => e.Scaling.ScaleForward(e.Get())),
                    Outputs.Count
                );

                var output = Method.Distribute(
                    input,
                    Inputs.Count
                );

                foreach (var o in Inputs.Zip(output, (a, b) => new { Input = a, Value = b }))
                {
                    o.Input.Set(o.Input.Scaling.ScaleForward(o.Value));
                }

                return Result.OK;
            }

            return Result.OK;
        }


    }
}

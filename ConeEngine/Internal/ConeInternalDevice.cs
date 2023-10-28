using ConeEngine.Internal.Actions;
using ConeEngine.Internal.BindNodes;
using ConeEngine.Model.Device;
using ConeEngine.Model.Entry.Action;
using ConeEngine.Model.Entry.Bind;
using ConeEngine.Model.Flow;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Internal
{
    public class ConeInternalDevice : Device
    {
        public ConeInternalDevice()
        {
            Name = "Built-in engine device";
            ID = "cone";
        }

        public override Result<CAction> CreateAction(Context ctx, string target, JObject config)
        {
            if (target == "toggle")
            {
                var min = config.Value<double>("min");
                var max = config.Value<double>("max");

                var nid = config.Value<string>("id");

                var act = new ToggleConeInternalAction(nid, min,max);

                return Result.VAL<CAction>(act);
            }

            if(target == "shell")
            {
                var cmd = config.Value<string>("command");

                var act = new ShellConeInternalAction(cmd);

                return Result.VAL<CAction>(act);
            }

            if(target == "key")
            {
                var act = new KeyConeInternalAction();

                return Result.VAL<CAction>(act);
            }

            if(target == "set")
            {
                var act = new SetInternalAction();

                return Result.VAL<CAction>(act);
            }

            return Result.Error<CAction>("Could not find matching internal action.");
        }
        public override Result<BindNode> CreateBindNode(Context ctx, JObject config)
        {
            var target = config.Value<string>("target");

            if(target == "random")
            {
                var bn = new RNGInternalBindNode();

                if (config.Value<double?>("min") is double min)
                    bn.Minimum = min;

                if (config.Value<double?>("max") is double max)
                    bn.Maximum = max;

                return Result.VAL<BindNode>(bn);
            }

            if(target == "time")
            {
                var bn = new TimeInternalBindNode();

                return Result.VAL<BindNode>(bn);
            }

            if(target == "once")
            {
                var bn = new OnceInternalBindNode();

                if (config.Value<double?>("start") is double start)
                    bn.Value = start;

                return Result.VAL<BindNode>(bn);
            }

            if(target == "debug")
            {
                var bn = new DebugInternalBindNode();

                return Result.VAL<BindNode>(bn);
            }

            return Result.Error<BindNode>("Could not find matching internal bind node.");
        }
    }
}

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
        public static Dictionary<string, Type> ActionDictionary = new()
        {
            { "toggle", typeof(ToggleConeInternalAction) },
            { "shell", typeof(ShellConeInternalAction) },
            { "key", typeof(KeyConeInternalAction) },
            { "set", typeof(SetInternalAction) },
            { "stop", typeof(StopConeInternalAction) },
        };

        public static Dictionary<string, Type> BindNodeDictionary = new()
        {
            {"random", typeof(RNGInternalBindNode)},
            {"time", typeof(TimeInternalBindNode)},
            {"once", typeof(OnceInternalBindNode)},
            {"debug", typeof(DebugInternalBindNode)},
        };

        public ConeInternalDevice()
        {
            Name = "Built-in engine device";
            ID = "cone";
        }

        public override Result<CAction> CreateAction(Context ctx, string target, JObject config)
        {
            if(ActionDictionary.ContainsKey(target))
            {
                var type = ActionDictionary[target];

                var obj = (CAction?)Activator.CreateInstance(type);

                if(obj is not null)
                {
                    return Result.VAL(obj);
                }
            }

            return Result.Error<CAction>("Could not find matching internal action.");
        }
        public override Result<BindNode> CreateBindNode(Context ctx, JObject config)
        {
            var target = config.Value<string>("target");

            if (target is not null && BindNodeDictionary.ContainsKey(target))
            {
                var type = BindNodeDictionary[target];

                var obj = (BindNode?)Activator.CreateInstance(type);

                if (obj is not null)
                {
                    return Result.VAL(obj);
                }
            }

            return Result.Error<BindNode>("Could not find matching internal bind node.");
        }
    }
}

using ConeEngine.Model.Entry.Action;
using ConeEngine.Model.Flow;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Internal.Actions
{
    public class ShellConeInternalAction : CAction
    {
        public string Command { get; set; } = "";

        public override Result Execute(Context ctx, object[] args)
        {
            var pi = new ProcessStartInfo(
                "cmd", string.Format($"/c{Command}", args)
                )
            {
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var p = new Process();
            p.StartInfo = pi;

            p.Start();

            return Result.OK;
        }

        public override void Deserialize(JObject config, Context ctx)
        {
            base.Deserialize(config, ctx);

            if (config.Value<string>("command") is string jcmd)
                Command = jcmd;
        }
    }
}

using ConeEngine.Model.Entry.Action;
using ConeEngine.Model.Flow;
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
        public string Command { get; set; }

        public ShellConeInternalAction(string command)
        {
            Command = command;
        }

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
    }
}

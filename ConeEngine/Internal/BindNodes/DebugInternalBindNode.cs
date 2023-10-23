using ConeEngine.Model.Entry.Bind;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Internal.BindNodes
{
    internal class DebugInternalBindNode : BindNode
    {
        public override double Get()
        {
            return 1;
        }

        public override void Set(double value)
        {
            Log.Information("Debug node value set to {0}", value);
        }

        public override bool HasPoll(bool reset = false)
        {
            return false;
        }
    }
}

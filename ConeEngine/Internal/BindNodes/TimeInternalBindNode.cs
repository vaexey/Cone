using ConeEngine.Model.Entry.Bind;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Internal.BindNodes
{
    public class TimeInternalBindNode : BindNode
    {
        public override double Get()
        {
            var t = TimeSpan.FromTicks(DateTime.Now.Ticks).TotalMilliseconds;

            //t = 499;

            return t;
        }

        public override void Set(double value)
        {
            throw new Exception("Cannot set random generator value.");
        }

        public override bool Diff()
        {
            return true;
        }
    }
}

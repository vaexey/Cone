using ConeEngine.Model.Entry.Bind;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Internal.BindNodes
{
    public class RNGInternalBindNode : BindNode
    {
        protected Random rnd = new();

        public double Minimum { get; set; } = 0;
        public double Maximum { get; set; } = 1;

        public override double Get()
        {
            return rnd.NextDouble() * (Maximum - Minimum) + Minimum;
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

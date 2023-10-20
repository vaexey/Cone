using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Model.Entry.Bind
{
    public class SimpleBindScaling : BindScaling
    {
        public double FromMin { get; set; }
        public double FromMax { get; set; }

        public double ToMin { get; set; }
        public double ToMax { get; set; }

        public override double ScaleForward(double source)
        {
            return (source - FromMin) / (FromMax - FromMin) * (ToMax - ToMin) + ToMin;
        }
        public override double ScaleBackward(double destination)
        {
            return (destination - ToMin) / (ToMax - ToMin) * (FromMax - FromMin) + FromMin;
        }
    }
}

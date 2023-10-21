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

        public double Modulo { get; set; } = 0;
        public int Round { get; set; } = -1;

        public override double ScaleForward(double source)
        {

            if (Modulo != 0)
                source = source % Modulo;

            source = (source - FromMin) / (FromMax - FromMin) * (ToMax - ToMin) + ToMin;

            if (Round >= 0)
                source = Math.Round(source, Round);

            return source;
        }
        public override double ScaleBackward(double destination)
        {
            return (destination - ToMin) / (ToMax - ToMin) * (FromMax - FromMin) + FromMin;
        }
    }
}

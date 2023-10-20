using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Model.Entry.Bind
{
    public class BindScaling
    {
        public virtual double ScaleForward(double source)
        {
            return source;
        }

        public virtual double ScaleBackward(double destination)
        {
            return destination;
        }
    }
}

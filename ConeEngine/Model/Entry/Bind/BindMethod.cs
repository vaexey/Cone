using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Model.Entry.Bind
{
    public abstract class BindMethod
    {
        public abstract double Combine(IEnumerable<double> sources, int count);
        public abstract IEnumerable<double> Distribute(double source, int count);
    }
}

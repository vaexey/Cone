using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Model.Entry.Bind
{
    public class BindMethod
    {
        public enum SimpleModeIn
        {
            SUM,
            AVERAGE,
            FIRST
        }

        public enum SimpleModeOut
        {
            REPEAT,
            SPLIT
        }

        public SimpleModeIn In = SimpleModeIn.SUM;
        public SimpleModeOut Out = SimpleModeOut.REPEAT;

        public virtual double Combine(IEnumerable<double> sources, int count)
        {
            switch (In)
            {
                case SimpleModeIn.SUM:
                    return sources.Sum();
                case SimpleModeIn.AVERAGE:
                    return sources.Sum() / sources.Count();

                default:
                    return sources.First();
            }
        }

        public virtual IEnumerable<double> Distribute(double source, int count)
        {
            var val = source;

            if (Out == SimpleModeOut.SPLIT)
                val = source / count;

            for (int i = 0; i < count; i++)
            {
                yield return val;
            }
        }
    }
}

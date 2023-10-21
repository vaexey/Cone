using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Model.Entry.Bind
{
    public class BindTrigger
    {
        public virtual double Minimum { get; set; } = 0;
        public virtual double Maximum { get; set; } = 1;
        public bool Enabled { get; set; } = false;

        public virtual bool Validate(double value)
        {
            return !Enabled || (CmpDbl(Minimum, value) && CmpDbl(value, Maximum));
        }

        protected bool CmpDbl(double smaller, double bigger)
        {
            return bigger >= smaller || Math.Abs(bigger - smaller) < 0.01;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Model.Entry.Bind
{
    public class BindTrigger
    {
        public virtual double Minimum { get; set; } = 1;
        public virtual double Maximum { get; set; } = 0;
        public bool Enabled { get; set; } = false;

        public virtual bool Validate(double value)
        {
            return !Enabled || (value >= Minimum && value <= Maximum);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Model.Entry.Bind
{
    public abstract class BindNode
    {
        public BindScaling Scaling { get; set; } = new();

        public BindScaling TriggerScaling { get; set; } = new();
        public BindTrigger Trigger { get; set; } = new();

        public abstract double Get();

        public abstract void Set(double value);

        protected bool hasChanged = false;
        public virtual bool Diff()
        {
            var r = hasChanged;

            hasChanged = false;

            return r;
        }

        protected void SetChanged(bool ch)
        {
            hasChanged = ch;
        }
    }
}

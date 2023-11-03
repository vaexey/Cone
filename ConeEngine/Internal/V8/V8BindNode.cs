using ConeEngine.Model.Entry.Bind;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Internal.V8
{
    public class V8BindNode : BindNode
    {
        public double Value { get; set; }

        public bool SetFlag { get; set; } = false;
        public bool PollFlag
        {
            get => hasPoll;
            set => hasPoll = value;
        }

        public override double Get()
        {
            return Value;
        }

        public override void Set(double value)
        {
            Value = value;
            SetFlag = true;
        }
    }
}

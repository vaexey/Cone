using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Model.Entry.Snapshot
{
    public class BindEntrySnapshot
    {
        public int[] ActivatedInputs { get; set; } = new int[0];
        public double ValueCombined { get; set; } = 0;
        public double ValueInputScaled { get; set; } = 0;
        public double ValueOutputScaled { get; set; } = 0;

        public double[] Combined { get; set; } = new double[0];
        public double[] Distribution { get; set; } = new double[0];

        public bool Triggered { get; set; } = false;

        public void Reset()
        {
            ActivatedInputs = new int[0];
            ValueCombined = 0;
            ValueInputScaled = 0;
            ValueOutputScaled = 0;

            Combined = new double[0];
            Distribution = new double[0];

            Triggered = false;

        }
    }
}

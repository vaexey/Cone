using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Model.Entry.Snapshot
{
    public class EventEntrySnapshot
    {
        public bool[] Triggers { get; set; } = new bool[0];
        public bool Triggered { get; set; } = false;

        public void Reset()
        {
            Triggers = new bool[0];
            Triggered = false;
        }
    }
}

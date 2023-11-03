using ConeEngine.Model.Entry.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Internal.V8
{
    public class V8EventHostObject
    {
        public readonly EventEntry entry;
        public readonly bool[] triggerValues;

        public V8EventHostObject(EventEntry entry, bool[] triggerValues)
        {
            this.entry = entry;
            this.triggerValues = triggerValues;
        }
    }
}

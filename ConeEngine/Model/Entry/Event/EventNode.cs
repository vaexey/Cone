using ConeEngine.Model.Flow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Model.Entry.Event
{
    public abstract class EventNode
    {
        public abstract bool Poll(Context ctx);
    }
}

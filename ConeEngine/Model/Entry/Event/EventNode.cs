using ConeEngine.Model.Flow;
using Newtonsoft.Json.Linq;
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

        public virtual void Deserialize(JObject config, Context ctx)
        { }
    }
}

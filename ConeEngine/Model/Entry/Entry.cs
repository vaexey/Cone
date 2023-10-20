using ConeEngine.Model.Flow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Model.Entry
{
    public abstract class Entry
    {
        public string? Name { get; set; }
        public string? ID { get; set; }
        public abstract EntryType? Type { get; }

        public virtual Result Enable(Context ctx)
        {
            return Result.OK;
        }
        public virtual Result Disable(Context ctx)
        {
            return Result.OK;
        }

        public abstract Result Update(Context ctx);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Model.Flow
{
    public class SchTask
    {
        public virtual Action<Context> Action { get; set; } = (Context ctx) => { };
        public virtual DateTime Time { get; set; }

        public virtual void Execute(Context ctx)
        {
            Action(ctx);
        }

        public virtual bool Check()
        {
            return Time.Subtract(DateTime.Now).Ticks <= 0;
        }
    }
}

using ConeEngine.Model.Flow;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Model.Entry.Action
{
    public abstract class CAction
    {
        public int Timeout { get; set; } = 0;

        protected DateTime enableTime = DateTime.Now;

        public virtual Result ExecuteSafe(Context ctx, object[] args)
        {
            if(ValidateTime())
            {
                SetTimeout();
                return Execute(ctx, args);
            }

            return Result.OK;
        }

        public abstract Result Execute(Context ctx, object[] args);

        public virtual bool ValidateTime()
        {
            return DateTime.Now.Subtract(enableTime).Ticks > 0;
        }

        public virtual void SetTimeout()
        {
            if(Timeout > 0)
            {
                enableTime = DateTime.Now.AddMilliseconds(Timeout);
            }
        }

        public virtual void Deserialize(JObject config, Context ctx)
        {
            Timeout = config.Value<int>("timeout");
        }
    }
}

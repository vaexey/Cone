using ConeEngine.Model.Device;
using ConeEngine.Model.Flow;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Plugin
{
    public abstract class Plugin : IPlugin
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract string ID { get; }

        public bool Enabled { get => enabled; }
        protected bool enabled = false;

        public virtual Result Disable(Context ctx) => Result.OK;

        public virtual Result Enable(Context ctx) => Result.OK;

        public virtual Result<Device> RegisterDevice(Context ctx, JObject config)
        {
            return Result.Error<Device>("Not implemented.");
        }

        public virtual Result Update(Context ctx) => Result.OK;
    }
}

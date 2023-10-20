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
    public interface IPlugin
    {
        public string Name { get; }
        public string Description { get; }
        public string ID { get; }
        public bool Enabled { get; }

        public Result Enable(Context ctx);
        public Result Disable(Context ctx);
        public Result Update(Context ctx);

        public Result<Device> RegisterDevice(Context ctx, JObject config);

    }
}

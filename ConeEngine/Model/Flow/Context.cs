using ConeEngine.Model.Entry;
using ConeEngine.Model.Entry.Bind;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Model.Flow
{
    public class Context
    {
        protected Engine engine;
        public Context(Engine engine)
        {
            this.engine = engine;
        }

        public Entry.Entry? GetEntry(string id)
        {
            return engine.Entries.Find(e => e.ID == id);
        }

        public T? GetEntry<T>(string id) where T : Entry.Entry
        {
            return GetEntry(id) as T;
        }

        public Device.Device? GetDevice(string id)
        {
            return engine.Devices.Find(d => d.ID == id);
        }

        public T? GetDevice<T>(string id) where T : Device.Device
        {
            return GetDevice(id) as T;
        }


        public BindNode InstantiateBindNode(JObject? config)
        {
            if (config == null)
                throw new Exception("Bind node config is null");

            var devid = config.Value<string>("device");
            if (devid is not string)
                throw new Exception("No device key found on a bind node");

            var dev = GetDevice(devid);

            if (dev is null)
                throw new Exception($"Could not find device {devid}");

            var bnm = dev.CreateBindNode(this, config);

            if (!bnm.IsOK || bnm.Value is null)
                throw new Exception(bnm.Message);

            var bn = bnm.Value;
            bn.Deserialize(config, this);

            return bn;
        }
    }
}

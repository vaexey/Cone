using ConeEngine.Model.Config;
using ConeEngine.Model.Entry.Action;
using ConeEngine.Model.Entry.Bind;
using ConeEngine.Model.Entry.Event;
using ConeEngine.Model.Entry.Snapshot;
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

        public EngineProperties GetProperties() => engine.Properties;

        public Entry.Entry? GetEntry(string id)
        {
            return engine.Entries.Find(e => e.ID == id);
        }

        public T? GetEntry<T>(string id) where T : Entry.Entry
        {
            return GetEntry(id) as T;
        }

        public Entry.Entry[] GetAllEntries()
        {
            return engine.Entries.ToArray();
        }

        public BindEntry? GetBindEntry(string id)
        {
            var ent = GetEntry(id);

            if (ent is not BindEntry)
                return null;

            return ent as BindEntry;
        }

        public Device.Device? GetDevice(string id)
        {
            return engine.Devices.Find(d => d.ID == id);
        }

        public Device.Device[] GetAllDevices()
        {
            return engine.Devices.ToArray();
        }

        public T? GetDevice<T>(string id) where T : Device.Device
        {
            return GetDevice(id) as T;
        }

        private Internal.V8Device? v8device;
        public Internal.V8Device GetV8Device()
        {
            if(v8device == null)
            {
                v8device = GetDevice<Internal.V8Device>("v8");
            }

            if (v8device == null)
                throw new Exception("V8 device does not exist!");

            return v8device;
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

        public CAction InstantiateAction(JObject? config)
        {
            if (config == null)
                throw new Exception("Action config is null");

            var devid = config.Value<string>("device");
            if (devid is not string)
                throw new Exception("No device key found on an action");

            var dev = GetDevice(devid);

            if (dev is null)
                throw new Exception($"Could not find device {devid}");

            var target = config.Value<string>("target");
            if (target is not string)
                throw new Exception("No target key found on an action");

            var actm = dev.CreateAction(this, target, config);

            if (!actm.IsOK || actm.Value is null)
                throw new Exception(actm.Message);

            var act = actm.Value;
            act.Deserialize(config, this);

            return act;
        }

        public EventNode InstantiateEventNode(JObject? config)
        {
            if (config == null)
                throw new Exception("Event node config is null");

            if (config["bind"] is JObject bej)
            {
                var be = InstantiateBindNode(bej);

                return new BindEventNode(be);
            }
            else
            {
                throw new Exception("Unknown event node type");
            }
        }
    }
}

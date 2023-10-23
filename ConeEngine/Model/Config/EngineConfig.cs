using ConeEngine.Internal;
using ConeEngine.Model.Entry.Action;
using ConeEngine.Model.Entry.Bind;
using ConeEngine.Model.Entry.Event;
using ConeEngine.Model.Flow;
using ConeEngine.Util;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Model.Config
{
    public class EngineConfig
    {
        public void Load(Engine e)
        {
            GenerateFromJSON5();

            Deserialize(e);
        }

        protected void GenerateFromJSON5()
        {
            //var p = Process.Start("node ", @".\..\..\..\..\..\ConeJSON\generateConfig.js");
            var p = Process.Start("node ", @"./compiler/generateConfig.js");

            p.WaitForExit();

            if(p.ExitCode != 0)
            {
                throw new Exception("Config compiler did not exit correctly.");
            }
        }

        protected void Deserialize(Engine e)
        {
            var path = "./config/config.json";
             var str = File.ReadAllText(path);
            var json = JObject.Parse(str);

            var devArray = json["devices"] as JArray;
            var entArray = json["entries"] as JArray;

            if (devArray is null || entArray is null)
                throw new Exception("Could not load config. Nonexistent main arrays.");

            Log.Debug("Registering devices...");

            foreach(var devj in devArray)
            {
                try
                {
                    Log.Verbose("Registering device {0}...", devj.Value<string>("name"));

                    var plname = devj.Value<string>("plugin");
                    var p = PluginLoader.Get(plname);

                    var dres = p.RegisterDevice(e.Context, (JObject)devj);

                    if (!dres.IsOK)
                        throw new Exception(dres.Message);

                    dres.Value.ID = devj.Value<string>("id");
                    dres.Value.Name = devj.Value<string>("name");

                    e.Devices.Add(dres.Value);
                }
                catch(Exception ex)
                {
                    Log.Error("Could not load device. Error: {0}", ex.Message);
                }
            }

            Internals.LinkInternals(e);

            Log.Information("Registered {0} devices.", e.Devices.Count);

            Log.Debug("Loading entries...");

            foreach(var entj in entArray)
            {
                try
                {
                    var type = entj.Value<string>("type");
                    var id = entj.Value<string>("id");

                    Log.Verbose("Registering entry...", id);
                    
                    Entry.Entry ent;

                    if (type == "bind")
                    {
                        ent = new BindEntry();
                    }
                    else if(type == "event")
                    {
                        ent = new EventEntry();
                    }
                    else
                    {
                        Log.Warning("Unknown type {0} for node {1}", type, id);

                        continue;
                    }

                    ent.Deserialize(entj as JObject, e.Context);

                    e.Entries.Add(ent);
                }
                catch(Exception ex)
                {
                    Log.Error("Could not load entry. Error: {0}", ex.Message);
                }
            }
        }

        protected EventEntry DeserializeEventEntry(Engine e, JToken entj)
        {
            var trj = entj["trigger"] as JArray;
            var acj = entj["actions"] as JArray;

            var trigs = trj.Select(t => DeserializeEventNode(e, t));
            var acts = acj.Select(a => DeserializeAction(e, a));

            var ee = new EventEntry();

            ee.Triggers = trigs.ToList();
            ee.Actions = acts.ToList();

            return ee;
        }

        protected EventNode DeserializeEventNode(Engine e, JToken bnj)
        {
            return null;
            //if (bnj["bind"] is JToken o)
            //{
            //    //var ben = new BindEventNode(DeserializeBindNode(e, o));

            //    if (bnj.Value<bool>("change") is bool change)
            //        ben.OnChange = change;

            //    return ben;
            //}

            //throw new Exception("Unknown event node type in " + bnj.ToString());
        }

        protected CAction DeserializeAction(Engine e, JToken acj)
        {
            var devid = acj.Value<string>("device");
            var dev = e.GetDevice(devid);
            var target = acj.Value<string>("target");

            var act = dev.CreateAction(e.Context, target, acj as JObject).Value;

            if (acj.Value<int>("timeout") is int timeout)
                act.Timeout = timeout;

            return act;
        }
    }
}

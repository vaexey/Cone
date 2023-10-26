using ConeEngine.Internal;
using ConeEngine.Model.Entry.Action;
using ConeEngine.Model.Entry.Bind;
using ConeEngine.Model.Entry.Event;
using ConeEngine.Model.Entry.Snapshot;
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
    public static class EngineConfig
    {
        public static void GenerateFromJSON5()
        {
            //var p = Process.Start("node ", @".\..\..\..\..\..\ConeJSON\generateConfig.js");
            var p = Process.Start("node ", @"./compiler/index.js");

            p.WaitForExit();

            if(p.ExitCode != 0)
            {
                throw new Exception("Config compiler did not exit correctly.");
            }
        }

        public static void LoadConfig(Engine e)
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

                    if (plname is not string)
                        throw new Exception("Device does not contain plugin key");

                    var p = PluginLoader.Get(plname);

                    var dres = p.RegisterDevice(e.Context, (JObject)devj);

                    if (!dres.IsOK || dres.Value is null)
                        throw new Exception(dres.Message);

                    dres.Value.ID = devj.Value<string>("id") ?? "";
                    dres.Value.Name = devj.Value<string>("name") ?? "";

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
                    if (entj is null || entj is not JObject)
                        throw new Exception("Invalid entry. Expected JSON object");

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

#pragma warning disable CS8604 // entj validated above
                    ent.Deserialize(entj as JObject, e.Context);
#pragma warning restore CS8604

                    e.Entries.Add(ent);
                }
                catch(Exception ex)
                {
                    Log.Error("Could not load entry. Error: {0}", ex.Message);
                }
            }
        }
    }
}

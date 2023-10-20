using ConeEngine.Model.Entry.Bind;
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
            var p = Process.Start("node ", @".\..\..\..\..\..\ConeJSON\generateConfig.js");

            p.WaitForExit();
        }

        protected void Deserialize(Engine e)
        {
            var path = "./config.json";
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

            Log.Information("Registered {0} devices.", e.Devices.Count);

            Log.Debug("Loading entries...");

            foreach(var entj in entArray)
            {
                try
                {
                    var name = entj.Value<string>("name");
                    var id = entj.Value<string>("id");
                    var type = entj.Value<string>("type");

                    Log.Verbose("Registering entry {0}...", name);

                    if(type == "bind")
                    {
                        var inj = entj["input"] as JArray;
                        var outj = entj["output"] as JArray;

                        var ins = inj.Select(bnj => DeserializeNode(e, bnj));
                        var outs = outj.Select(bnj => DeserializeNode(e, bnj));

                        var bn = new BindEntry()
                        {
                            Name = name,
                            ID = id,
                            Inputs = ins.ToList(),
                            Outputs = outs.ToList()
                        };

                        if (entj.Value<string>("direction") is string dir)
                        {
                            if (dir == "both")
                                bn.Direction = BindDirection.BOTH;
                        }

                        e.Entries.Add(bn);
                    }
                    else
                    {
                        Log.Warning("Unknown type {0} for node {1}", type, name);
                    }
                }
                catch(Exception ex)
                {
                    Log.Error("Could not load entry. Error: {0}", ex.Message);
                }
            }
        }

        protected BindNode DeserializeNode(Engine e, JToken bnj)
        {
            var devid = bnj.Value<string>("device");
            var dev = e.GetDevice(devid);

            var bn = dev.CreateBindNode(e.Context, (JObject)bnj).Value;

            if (bnj.Value<JObject>("scale") is JObject scalej)
            {
                var sfrom = scalej["from"].Select(x => double.Parse(x.ToString()));
                var sto = scalej["to"].Select(x => double.Parse(x.ToString()));

                bn.Scaling = new SimpleBindScaling()
                {
                    FromMin = sfrom.First(),
                    FromMax = sfrom.Last(),
                    ToMin = sto.First(),
                    ToMax = sto.Last()
                };
            }

            return bn;
        }
    }
}

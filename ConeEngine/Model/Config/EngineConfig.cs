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
                    var name = entj.Value<string>("name");
                    var id = entj.Value<string>("id");
                    var type = entj.Value<string>("type");

                    Entry.Entry ent;

                    Log.Verbose("Registering entry {0}...", name);

                    if (type == "bind")
                    {
                        ent = DeserializeBindEntry(e, entj);
                    }
                    else if(type == "event")
                    {
                        ent = DeserializeEventEntry(e, entj);
                    }
                    else
                    {
                        Log.Warning("Unknown type {0} for node {1}", type, name);

                        continue;
                    }

                    ent.Name = name;
                    ent.ID = id;

                    e.Entries.Add(ent);
                }
                catch(Exception ex)
                {
                    Log.Error("Could not load entry. Error: {0}", ex.Message);
                }
            }
        }

        protected BindEntry DeserializeBindEntry(Engine e, JToken entj)
        {
            var inj = entj["input"] as JArray;
            var outj = entj["output"] as JArray;

            var ins = inj.Select(bnj => DeserializeBindNode(e, bnj));
            var outs = outj.Select(bnj => DeserializeBindNode(e, bnj));

            var bn = new BindEntry()
            {
                Inputs = ins.ToList(),
                Outputs = outs.ToList()
            };

            if (entj.Value<string>("direction") is string dir)
            {
                if (dir == "both")
                    bn.Direction = BindDirection.BOTH;
            }

            if (entj.Value<string>("policy") is string pol)
            {
                if (pol == "change")
                    bn.Policy = BindPolicy.ON_CHANGE;
            }

            return bn;
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
            if (bnj["bind"] is JToken o)
            {
                var ben = new BindEventNode(DeserializeBindNode(e, o));

                if (bnj.Value<bool>("change") is bool change)
                    ben.OnChange = change;

                return ben;
            }

            throw new Exception("Unknown event node type in " + bnj.ToString());
        }

        protected BindNode DeserializeBindNode(Engine e, JToken bnj)
        {
            var devid = bnj.Value<string>("device");
            var dev = e.GetDevice(devid);

            var bn = dev.CreateBindNode(e.Context, (JObject)bnj).Value;

            if (bnj.Value<JObject>("scale") is JObject scalej)
            {
                bn.Scaling = ParseScaling(scalej);
            }

            if(bnj.Value<JObject>("trigger") is JObject trigj)
            {
                bn.Trigger = ParseTrigger(trigj);

                if (trigj["scale"] is JObject tscalej)
                    bn.TriggerScaling = ParseScaling(tscalej);
            }

            return bn;
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

        protected BindScaling ParseScaling(JObject scalej)
        {
            var type = "";

            if (scalej.Value<string?>("type") is string t)
                type = t;


            var sfrom = scalej["from"].Select(x => double.Parse(x.ToString()));
            var sto = scalej["to"].Select(x => double.Parse(x.ToString()));

            var sc = new SimpleBindScaling()
            {
                FromMin = sfrom.First(),
                FromMax = sfrom.Last(),
                ToMin = sto.First(),
                ToMax = sto.Last()
            };

            if (scalej.Value<double>("mod") is double mod)
                sc.Modulo = mod;

            if (scalej.Value<int>("round") is int round)
                sc.Round = round;

            // Simple default

            return sc;
        }

        protected BindTrigger ParseTrigger(JObject btj)
        {
            var bt = new BindTrigger();

            if(btj.Value<bool>("enabled") is bool enabled)
                bt.Enabled = enabled;

            if (btj.Value<double>("min") is double min)
                bt.Minimum = min;

            if (btj.Value<double>("max") is double max)
                bt.Maximum = max;

            return bt;
        }
    }
}

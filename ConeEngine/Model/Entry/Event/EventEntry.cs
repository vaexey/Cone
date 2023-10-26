﻿using ConeEngine.Model.Entry.Action;
using ConeEngine.Model.Entry.Bind;
using ConeEngine.Model.Entry.Snapshot;
using ConeEngine.Model.Flow;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Model.Entry.Event
{
    public class EventEntry : Entry
    {
        public override EntryType? Type => EntryType.EVNT;
        public List<EventNode> Triggers { get; set; } = new();
        public List<CAction> Actions { get; set; } = new();

        public EventEntrySnapshot LastSnapshot = new();

        public override Result Update(Context ctx)
        {
            lock (LastSnapshot)
            {
                LastSnapshot.Reset();

                var triggers = Triggers.Select(t => t.Poll(ctx) ? 1 : 0);
                var points = triggers.Sum();

                var valid = points == Triggers.Count;

                if (valid)
                {
                    Log.Information("Event triggered!");

                    foreach (var a in Actions)
                        a.ExecuteSafe(ctx, new object[] { });
                }

                LastSnapshot.Triggers = triggers.Select(i => i > 0).ToArray();
                LastSnapshot.Triggered = valid;
            }

            return Result.OK;
        }

        public override void Deserialize(JObject config, Context ctx)
        {
            var trj = config["trigger"] as JArray;
            var acj = config["actions"] as JArray;

            if (trj is null || acj is null)
                throw new Exception($"Entry {ID} does not have trigger/action");

            Triggers = trj.Select(t => ctx.InstantiateEventNode(t as JObject)).ToList();
            Actions = acj.Select(a => ctx.InstantiateAction(a as JObject)).ToList();
        }
    }
}

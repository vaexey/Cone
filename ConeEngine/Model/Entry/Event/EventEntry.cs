using ConeEngine.Model.Entry.Action;
using ConeEngine.Model.Entry.Bind;
using ConeEngine.Model.Flow;
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
        public List<EventNode> Triggers { get; set; } = new();
        public List<CAction> Actions { get; set; } = new();
        public override EntryType? Type => EntryType.EVNT;

        public override Result Update(Context ctx)
        {
            var points = Triggers.Select(t => t.Poll(ctx) ? 1 : 0).Sum();

            var valid = points == Triggers.Count;

            if(valid)
            {
                Log.Information("Event triggered!");

                foreach (var a in Actions)
                    a.ExecuteSafe(ctx, new object[] { });
            }

            return Result.OK;
        }
    }
}

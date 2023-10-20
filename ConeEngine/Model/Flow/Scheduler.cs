using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Model.Flow
{
    public class Scheduler
    {
        public List<SchTask> Tasks { get; set; } = new();

        public virtual void Update(Context ctx)
        {
            var valid = Tasks.Where(t => t.Check()).ToArray();
            Tasks.RemoveAll(t => valid.Contains(t));

            foreach (var t in valid)
                t.Execute(ctx);
        }

        public virtual void AddScheduledTask(Action<Context> action, DateTime time)
        {
            var t = new SchTask()
            {
                Action = action,
                Time = time
            };

            Tasks.Add(t);
        }

        public virtual void AddScheduledTask(Action action, DateTime time)
        {
            AddScheduledTask((Context ctx) => action(), time);
        }

        public virtual void AddScheduledTask(Action<Context> action, int delayMs)
        {
            AddScheduledTask(action, DateTime.Now.Add(TimeSpan.FromMilliseconds(delayMs)));
        }

        public virtual void AddScheduledTask(Action action, int delayMs)
        {
            AddScheduledTask((Context ctx) => action(), delayMs);
        }
    }
}

using ConeEngine.Model.Config;
using ConeEngine.Model.Device;
using ConeEngine.Model.Entry;
using ConeEngine.Model.Flow;
using ConeEngine.Plugin;
using Serilog;

namespace ConeEngine
{
    public class Engine
    {
        /// <summary>
        /// Registered devices
        /// </summary>
        public List<Device> Devices { get; set; } = new();

        /// <summary>
        /// Registered entries
        /// </summary>
        public List<Entry> Entries { get; set; } = new();

        public EngineConfig Config { get; set; } = new();

        public Scheduler Scheduler { get; set; } = new();

        /// <summary>
        /// Engine context used for hermetization
        /// </summary>
        public Context Context { get; set; }

        public Engine()
        {
            Context = new(this);
        }

        public Result Initialize()
        {
            try
            {
                Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
                Log.Information("ConeEngine initialization begin.");

                Log.Information("Available plugins: {0}", PluginLoader.Plugins.Count);

                Log.Information("Loading config...");
                Config.Load(this);

                Log.Information("Enabling plugins...");
                PluginLoader.Enable(Context);

                Scheduler.AddScheduledTask(TryEnableTask, 1000);
            }
            catch(Exception ex)
            {
                Log.Fatal("Initialization error:");
                Log.Fatal("{0}", ex.ToString());

                return Result.Error(ex);
            }

            return Result.OK;
        }

        public void Update()
        {
            foreach (var pl in PluginLoader.Plugins)
                pl.Update(Context);

            foreach (var ent in Entries)
                ent.Update(Context);

            Scheduler.Update(Context);
        }

        public Device GetDevice(string id)
        {
            return Devices.Find(d => d.ID == id);
        }

        protected void TryEnableTask()
        {
            PluginLoader.TryEnable(Context);

            Scheduler.AddScheduledTask(TryEnableTask, 1000);
        }
    }
}
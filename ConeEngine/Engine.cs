using ConeEngine.Model.Config;
using ConeEngine.Model.Device;
using ConeEngine.Model.Entry;
using ConeEngine.Model.Flow;
using ConeEngine.Panel;
using ConeEngine.Plugin;
using Serilog;

namespace ConeEngine
{
    public class Engine
    {
        public bool IsRunning { get => running; }
        bool running = false;

        /// <summary>
        /// Registered devices
        /// </summary>
        public List<Device> Devices { get; set; } = new();

        /// <summary>
        /// Registered entries
        /// </summary>
        public List<Entry> Entries { get; set; } = new();
        
        /// <summary>
        /// Cone task scheduler
        /// </summary>
        public Scheduler Scheduler { get; set; } = new();

        /// <summary>
        /// Engine context used for hermetization
        /// </summary>
        public Context Context { get; set; }

        /// <summary>
        /// ConePanel backend server
        /// </summary>
        public PanelServer Panel { get; set; }

        /// <summary>
        /// Basic engine properties
        /// </summary>
        public EngineProperties Properties { get; set; } = new();

        public Engine()
        {
            Context = new(this);
            Panel = new(Context);
        }

        public void LoadLogger()
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Console()
            .WriteTo.File("log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();
        }

        public void LoadProperties()
        {
            Properties = EngineProperties.Load();

            EngineProperties.Save(Properties);
        }

        public void LoadPlugins()
        {
            Log.Information("Loading plugins...");
            
            PluginLoader.LoadPlugins(Properties.PluginsDirectory);

            Log.Debug("Found {0} plugins", PluginLoader.Plugins.Count);
        }

        public Result Initialize()
        {
            try
            {
                Log.Information("ConeEngine initialization begin.");

                Log.Information("Available plugins: {0}", PluginLoader.Plugins.Count);

                Log.Information("Loading config...");
                EngineConfigurator.GenerateFromJSON5(this);
                EngineConfigurator.LoadConfig(this);

                Log.Information("Enabling plugins...");
                PluginLoader.Enable(Context);

                Scheduler.AddScheduledTask(TryEnableTask, 1000);

                Log.Information("Initializing V8 engine...");
                Context.GetV8Device().Initialize(Context);

                Log.Information("Starting panel server...");

                _ = Panel.Run();

                running = true;
            }
            catch(Exception ex)
            {
                Log.Fatal("Initialization error:");
                Log.Fatal("{0}", ex.ToString());

                return Result.Error(ex);
            }

            return Result.OK;
        }

        public void Unload()
        {
            PluginLoader.Disable(Context);

            Devices.Clear();
            Entries.Clear();

            Scheduler.Tasks.Clear();

            Panel.Stop().Wait();

            running = false;
        }

        public void Update()
        {
            if (!PluginLoader.AreAllPluginsEnabled())
                throw new Exception("At least one plugin is not enabled");

            foreach (var pl in PluginLoader.Plugins)
                pl.Update(Context);

            foreach (var ent in Entries)
                ent.Update(Context);

            Context.GetV8Device().Update();

            Scheduler.Update(Context);
        }

        protected void TryEnableTask()
        {
            PluginLoader.TryEnable(Context);

            Scheduler.AddScheduledTask(TryEnableTask, 1000);
        }
    }
}
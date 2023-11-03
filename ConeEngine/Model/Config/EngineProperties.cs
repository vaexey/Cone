using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Model.Config
{
    public class EngineProperties
    {
        public const string PropertiesPath = "./cone.json";

        public string CompilerExecutable { get; } = "node";
        public string CompilerArgs { get; } = "./compiler/index.js";

        public string ConfigPath { get; } = "./config/config.json";
        
        public string PluginsDirectory { get; } = "./plugins";

        public bool AutoRestart = true;

        public static EngineProperties Load()
        {
            try
            {
                if (!File.Exists(PropertiesPath))
                {
                    Log.Information("Properties file does not exist and will be created.");

                    return new();
                }

                var obj = JsonConvert
                    .DeserializeObject<EngineProperties>(
                        File.ReadAllText(PropertiesPath)
                    );

                if (obj is null)
                    throw new Exception("Null deserialization object received");

                return obj;
            }
            catch (Exception ex)
            {
                Log.Warning("Could not load properties: {0}", ex.Message);
            }

            return new();
        }

        public static void Save(EngineProperties props)
        {
            try
            {
                var output = JsonConvert.SerializeObject(props);

                File.WriteAllText(PropertiesPath, output);
            }
            catch(Exception ex)
            {
                Log.Error("Could not save properties: {0}", ex.Message);
            }
        }
    }
}

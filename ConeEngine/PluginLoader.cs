using ConeEngine.Model.Flow;
using ConeEngine.Plugin;
using ConeEngine.Util;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine
{
    public class PluginLoader
    {
        public static List<IPlugin> Plugins = new();
        public static Dictionary<string, IPlugin> PluginMap = new();
        protected static bool loaded = false;

        public static void Enable(Context ctx)
        {
            foreach(var p in Plugins)
            {
                var r = ExWrapper.Wrap(() => p.Enable(ctx));

                if (!r)
                    Log.Warning("Plugin {0} had an error when enabling: {1}", p.Name, r.Message);
                else
                    Log.Debug("Plugin {0} enabled", p.Name);
            }
        }

        public static void TryEnable(Context ctx)
        {
            foreach(var p in Plugins.Where(p => !p.Enabled))
            {
                Log.Verbose("Trying to enable still waiting plugin {0}...", p.Name);

                var r = ExWrapper.Wrap(() => p.Enable(ctx));

                if (!r)
                    Log.Verbose("Plugin {0} had an error when retrying enabling: {1}", p.Name, r.Message);
                else
                    Log.Warning("Plugin {0} enabled after retry.", p.Name);
            }
        }

        public static void Disable(Context ctx)
        {
            foreach (var p in Plugins)
            {
                var r = ExWrapper.Wrap(() => p.Disable(ctx));

                if (!r)
                    Log.Warning("Plugin {0} had an error when disabling: {1}", p.Name, r.Message);
                else
                    Log.Debug("Plugin {0} disabled", p.Name);
            }
        }

        public static IPlugin Get(string id)
        {
            if (!PluginMap.ContainsKey(id))
                throw new Exception($"Plugin with id ${id} does not exist.");

            return PluginMap[id];
        }

        public static void LoadPlugins()
        {
            if (loaded)
                throw new Exception("Plugins have been already loaded.");

            foreach (
                var p in Directory.GetFiles("./plugins")
                .Where(p => Path.GetFileNameWithoutExtension(p)
                .ToLower()
                .Contains("cone"))
            )
            {
                LoadPlugin(p);
            }

            loaded = true;
        }

        public static void LoadPlugin(string path)
        {
            var a = LoadPluginAssembly(path);
            var pl = LoadPluginsFromAssembly(a);

            foreach (var p in pl)
            {
                var id = p.ID;

                if (PluginMap.ContainsKey(id))
                    throw new Exception("Plugin ID conflict.");

                Plugins.Add(p);
                PluginMap[id] = p;

                Log.Debug("Loaded plugin {0}", p.Name);
            }
        }

        protected static Assembly LoadPluginAssembly(string path)
        {
            PluginLoadContext loadCtx = new PluginLoadContext(path);

            return loadCtx.LoadFromAssemblyName(
                new AssemblyName(Path.GetFileNameWithoutExtension(path))
            );
        }

        protected static IEnumerable<IPlugin> LoadPluginsFromAssembly(Assembly asm)
        {
            int count = 0;

            foreach (Type t in asm.GetTypes())
            {
                if (typeof(IPlugin).IsAssignableFrom(t))
                {
                    IPlugin? p = Activator.CreateInstance(t) as IPlugin;

                    if (p is not null)
                    {
                        count++;
                        yield return p;
                    }
                }
            }

            if (count == 0)
            {
                //
            }
        }
    }
}

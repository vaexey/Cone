using ConeEngine;

Engine e = new();

PluginLoader.LoadPlugins();
e.Initialize();

while(true)
{
    e.Update();
    Thread.Sleep(1);
}
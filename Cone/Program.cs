using ConeEngine;

Engine e = new();

PluginLoader.LoadPlugins();

if(!e.Initialize().IsOK)
{
    return;
}

while (true)
{
    e.Update();
    Thread.Sleep(1);
}
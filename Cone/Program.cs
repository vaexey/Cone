using ConeEngine;
using Serilog;

Engine e = new();

e.LoadLogger();
e.LoadProperties();

while (true)
{
#if !DEBUG
    try
    {
#endif
    e.LoadPlugins();

    var init = e.Initialize();

    if (!init.IsOK)
    {
        throw new Exception(init.Message);
    }

    while (e.IsRunning)
    {
        e.Update();
        Thread.Sleep(1);
    }
#if !DEBUG
    }
    catch(Exception ex)
    {
        Log.Fatal("Engine has stopped: {0}", ex.Message);

        e.Unload();
    }
#endif

    if (!e.Properties.AutoRestart)
        break;

    Log.Information("Restarting...");

    Thread.Sleep(1000);
}
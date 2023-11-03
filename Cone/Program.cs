using ConeEngine;
using Serilog;

Engine e = new();

e.LoadLogger();
e.LoadProperties();

while (true)
{

    try
    {
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
    }
    catch(Exception ex)
    {
        Log.Fatal("Engine has stopped: {0}", ex.Message);

        e.Unload();
    }

    if (!e.Properties.AutoRestart)
        break;

    Log.Information("Restarting...");

    Thread.Sleep(1000);
}
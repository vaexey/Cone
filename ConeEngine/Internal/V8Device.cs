using ConeEngine.Internal.V8;
using ConeEngine.Model.Device;
using ConeEngine.Model.Entry.Event;
using ConeEngine.Model.Flow;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Internal
{
    public class V8Device : Device
    {
        public V8ScriptEngine ScriptEngine { get; }
        private V8Context? v8context;

        // TODO: Dispose

        public V8Device()
        {
            Name = "Built-in script device";
            ID = "v8";

            ScriptEngine = new();
        }

        public void Initialize(Context ctx)
        {
            v8context = new(ctx, this);

            ScriptEngine.AddHostObject("lib", new HostTypeCollection("mscorlib", "System.Core"));
            ScriptEngine.AddHostType("Log", typeof(Log));
            ScriptEngine.AddHostObject("Context", ctx);
            ScriptEngine.AddHostObject("V8", v8context);

            ScriptEngine.Execute(@"Log.Information('V8 engine initialized');");

            if (!File.Exists("./config/script.js"))
            {
                throw new Exception("V8 script file could not be found!");
            }

            v8context.Require("./config/script.js");

            var updateType = ScriptEngine.Evaluate("typeof update");

            if (updateType is string and "function")
            {
                return;
            }

            throw new Exception("Could not find a matching update function");
        }

        public void Update()
        {
            ScriptEngine.Execute("update();");
        }

        public V8ValidatorResult[] ValidateEvent(EventEntry ee, bool[] triggerValues)
        {
            var eho = new V8EventHostObject(ee, triggerValues);

            ScriptEngine.AddHostObject("$event", eho);

            //dynamic result = ScriptEngine.Evaluate(
            //    @"(() => {
            //        let entry = cone_temp_object_instance1;
            //        let triggers = cone_temp_object_instance2;
            //        let args = [];
            //        let valid = false;
            //        " + ee.Code + @"
            //        let result = {}
            //        result.args = args;
            //        result.valid = valid;
            //        return result;
            //    })()");

            dynamic rawResults = ScriptEngine.Evaluate(ee.Code);

            var validArr = new bool[ee.Actions.Count];
            var argsArr = new string[ee.Actions.Count][];
            
            for(var i = 0; i < ee.Actions.Count; i++)
            {
                dynamic obj = rawResults[i];

                validArr[i] = (bool)obj.valid;

                int aLen = obj.args.length;

                argsArr[i] = new string[aLen];

                for(int j = 0; j < aLen; j++)
                {
                    argsArr[i][j] = (string)obj.args[j];
                }
            }

            var results = new List<V8ValidatorResult>();

            for (var i = 0; i < ee.Actions.Count; i++)
                results.Add(new(validArr[i], argsArr[i]));

            return results.ToArray();
        }
    }
}

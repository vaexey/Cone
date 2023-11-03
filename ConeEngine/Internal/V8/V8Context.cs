using ConeEngine.Model.Entry.Bind;
using ConeEngine.Model.Flow;
using Microsoft.ClearScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Internal.V8
{
    public class V8Context
    {
        private Context ctx;
        private V8Device v8;

        public PropertyBag InputNodes { get; } = new();
        public PropertyBag OutputNodes { get; } = new();

        public PropertyBag RawInputNodes { get; } = new();
        public PropertyBag RawOutputNodes { get; } = new();

        public V8Context(Context ctx, V8Device v8)
        {
            this.ctx = ctx;
            this.v8 = v8;

            GatherEntries();
        }

        private void GatherEntries()
        {
            var bindEntries = ctx.GetAllEntries()
                .Where(e => e is BindEntry)
                .Select(e => (BindEntry)e);

            foreach(var be in bindEntries)
            {
                var inputs = be.Inputs
                    .Where(e => e is V8BindNode)
                    .Select(e => (V8BindNode)e)
                    .ToArray();

                var outputs = be.Outputs
                    .Where(e => e is V8BindNode)
                    .Select(e => (V8BindNode)e)
                    .ToArray();

                InputNodes[be.ID] = inputs;
                OutputNodes[be.ID] = outputs;

                RawInputNodes[be.ID] = be.Inputs.ToArray();
                RawOutputNodes[be.ID] = be.Outputs.ToArray();
            }
        }

        public void Require(string path)
        {
            var text = File.ReadAllText(path);

            v8.ScriptEngine.Execute(text);
        }
    }
}

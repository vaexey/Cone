using ConeEngine.Model.Entry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Model.Flow
{
    public class Context
    {
        protected Engine engine;
        public Context(Engine engine)
        {
            this.engine = engine;
        }

        public Entry.Entry GetEntry(string id)
        {
            return engine.Entries.Find(e => e.ID == id);
        }

        public T GetEntry<T>(string id) where T : Entry.Entry
        {
            return engine.Entries.Find(e => e.ID == id) as T;
        }
    }
}

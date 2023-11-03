using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Internal
{
    public static class Internals
    {
        public static void LinkInternals(Engine e)
        {
            e.Devices.Add(new ConeInternalDevice());
            e.Devices.Add(new V8Device());
        }
    }
}

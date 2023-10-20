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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Internal.V8
{
    public class V8ValidatorResult
    {
        public bool Valid { get; set; }
        public string[] Args { get; set; }

        public V8ValidatorResult(bool valid, string[] args)
        {
            Valid = valid;
            Args = args;
        }
    }
}

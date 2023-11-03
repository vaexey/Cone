using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Model.Flow
{
    public class StopException : Exception
    {
        public StopException() { }

        public StopException(string message) : base(message) { }
    }
}

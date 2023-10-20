using ConeEngine.Model.Entry.Bind;
using ConeEngine.Model.Flow;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Model.Device
{
    public abstract class Device
    {
        public string Name { get; set; }
        public string ID { get; set; }

        public virtual Result<BindNode> CreateBindNode(Context ctx, JObject config)
        {
            return Result.Error<BindNode>("Not implemented.");
        }
    }
}

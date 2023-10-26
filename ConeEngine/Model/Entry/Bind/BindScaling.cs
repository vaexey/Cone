using ConeEngine.Model.Flow;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Model.Entry.Bind
{
    public class BindScaling
    {
        public double FromMin { get; set; } = 0;
        public double FromMax { get; set; } = 1;

        public double ToMin { get; set; } = 0;
        public double ToMax { get; set; } = 1;

        public double Modulo { get; set; } = 0;
        public int Round { get; set; } = -1;

        public virtual double ScaleForward(double source)
        {

            if (Modulo != 0)
                source = source % Modulo;

            source = (source - FromMin) / (FromMax - FromMin) * (ToMax - ToMin) + ToMin;

            if (Round >= 0)
                source = Math.Round(source, Round);

            return source;
        }
        public virtual double ScaleBackward(double destination)
        {
            return (destination - ToMin) / (ToMax - ToMin) * (FromMax - FromMin) + FromMin;
        }
        public virtual void Deserialize(JObject config, Context ctx)
        {
            if(config["from"] is JArray jfrom)
            {
                var sfrom = jfrom.Select(x => double.Parse(x.ToString()));

                FromMin = sfrom.First();
                FromMax = sfrom.Last();
            }

            if(config["to"] is JArray jto)
            {
                var sto = jto.Select(x => double.Parse(x.ToString()));

                ToMin = sto.First();
                ToMax = sto.Last();
            }

            Modulo = config.Value<double>("modulo");
            Round = config.Value<int>("round");
        }
    }
}

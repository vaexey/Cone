﻿using ConeEngine.Model.Entry.Bind;
using ConeEngine.Model.Flow;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Internal.BindNodes
{
    public class OnceInternalBindNode : BindNode
    {
        public double Value { get; set; } = 0;
        public bool DiffValue { get; set; } = true;

        public override double Get()
        {
            return Value;
        }

        public override void Set(double value)
        {
            Value = value;

            DiffValue = true;
        }

        public override bool HasPoll(bool reset = false)
        {
            if(DiffValue)
            {
                if(reset)
                {
                    DiffValue = false;
                }

                return true;
            }

            return false;
        }

        public override void Deserialize(JObject config, Context ctx)
        {
            base.Deserialize(config, ctx);

            Value = config.Value<double>("start");
        }
    }
}

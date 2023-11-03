using ConeEngine.Model.Entry.Action;
using ConeEngine.Model.Flow;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Internal.Actions
{
    public class KeyConeInternalAction : CAction
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        protected const int KEYEVENTF_KEYDOWN = 0x0000;
        protected const int KEYEVENTF_EXTENDEDKEY = 0x0001;
        protected const int KEYEVENTF_KEYUP = 0x0002;

        public byte KeyCode { get; set; } = 0;
        public bool Down { get; set; } = true;
        public bool Extended { get; set; } = false;

        public override Result Execute(Context ctx, object[] args)
        {
            keybd_event(
                KeyCode,
                0,
                (Extended ? KEYEVENTF_EXTENDEDKEY : 0) +
                (Down ? KEYEVENTF_KEYDOWN : KEYEVENTF_KEYUP),
                0
            );

            return Result.OK;
        }

        public override void Deserialize(JObject config, Context ctx)
        {
            base.Deserialize(config, ctx);

            if(config.ContainsKey("ext"))
            {
                Extended = config.Value<bool>("ext");
            }

            if (config.ContainsKey("up"))
            {
                Down = false;
                KeyCode = config.Value<byte>("up");

                return;
            }

            if (config.ContainsKey("down"))
            {
                Down = true;
                KeyCode = config.Value<byte>("down");

                return;
            }

            throw new Exception("Could not deserialize Key action");
        }
    }
}

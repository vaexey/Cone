using ConeEngine.Model.Entry.Action;
using ConeEngine.Model.Flow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConeEngine.Internal.Actions
{
    public class StopConeInternalAction : CAction
    {
        public override Result Execute(Context ctx, object[] args)
        {
            throw new StopException();
        }
    }
}

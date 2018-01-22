using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpolation.Controller.Interface
{
    interface IInterpolateController
    {
        IList<InterpolationData> Strategy(InterpolationState ips);
    }
}

using Interpolation.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpolation.Controller.Interface
{
    interface IScanController
    {
        void setTimeGap(TimeGap timeGap);

        //调用此函数前，必须先调用setTimeGap
        void scanTask();
    }
}

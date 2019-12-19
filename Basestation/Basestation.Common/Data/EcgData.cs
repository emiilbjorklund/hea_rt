using System;
using System.Collections.Generic;
using System.Text;

namespace Basestation.Common.Data
{
    public class EcgData : DataMessage
    {
        public double Ll_Ra { get; set; }
        public double La_Ra { get; set; }
        public double Vx_Rl { get; set; }
        public double Ll_La => Ll_Ra - La_Ra;
    }
}

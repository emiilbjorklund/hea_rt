using Basestation.Common.Data;
using Basestation.Common.gRPC;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Basestation.MobileAppCommunication.Relay
{
    public class ArrythmiaSubscriber : DataSubscriber<ArrythmiaResponse>
    {
        public ArrythmiaSubscriber(string address) : base(address)
        {

        }

        public override Task ReadStream()
        {
            throw new NotImplementedException();
        }
    }
}

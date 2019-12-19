using Basestation.Common.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basestation.MobileAppCommunication.Relay
{
    public class RelayManager : IRelayManager
    {
        public HeartrateRelay Heartrate { get; set; }
        public ArrythmiaRelay Arrythmia { get; set; }


        public RelayManager(MobileAppCommunicationCapability capability)
        {
            var hrs = capability.GetHeartrateSources();
             if (hrs.Count > 0)
                Heartrate = new HeartrateRelay(hrs);
            var atgs = capability.GetArrythmiaTargets();
            if (atgs.Count > 0)
                Arrythmia = new ArrythmiaRelay(atgs);
        }
    }
}

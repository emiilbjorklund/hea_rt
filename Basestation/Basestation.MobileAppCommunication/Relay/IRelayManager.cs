using System;
using System.Collections.Generic;
using System.Text;

namespace Basestation.MobileAppCommunication.Relay
{
    public interface IRelayManager
    {
        HeartrateRelay Heartrate { get; set; }

        ArrythmiaRelay Arrythmia { get; set; }
    }
}

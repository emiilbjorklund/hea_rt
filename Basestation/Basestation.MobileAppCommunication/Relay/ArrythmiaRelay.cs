using Basestation.Common.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Basestation.MobileAppCommunication.Relay
{
    public class ArrythmiaRelay
    {
        private int sourceIndex;

        public ArrythmiaRelay(List<Capability> atgs)
        {
            foreach (var hr in atgs)
            {
                //HeartrateSources.Add(new HeartrateSubscriber(hr.Service.Address));
            }
            sourceIndex = 0;
        }


        public List<ArrythmiaSubscriber> ArrythmiaTargets { get; } = new List<ArrythmiaSubscriber>();
    }
}

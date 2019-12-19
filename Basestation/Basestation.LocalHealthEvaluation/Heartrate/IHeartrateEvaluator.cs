using Basestation.Common.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;

namespace Basestation.LocalHealthEvaluation.Heartrate
{
    public interface IHeartrateEvaluator
    {
        Channel<HeartrateData> AddSubscriber();
        void RemoveSubscriber(Channel<HeartrateData> channel);
    }
}

using Basestation.Common.Abstractions;
using Basestation.Common.Data;
using Basestation.Common.gRPC;
using LocalHealthEvaluation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using static LocalHealthEvaluation.LphesHeartrate;

namespace Basestation.MobileAppCommunication.Relay
{
    public class HeartrateRelay : DataStreamer<HeartrateData>
    {
        private Timer _swapTimer;
        private int sourceIndex;

        public HeartrateRelay(List<Capability> hrs)
        {
            foreach (var hr in hrs)
            {
                HeartrateSources.Add(new HeartrateSubscriber(hr.Service.Address));
            }
            sourceIndex = 0;
            //HeartrateSources.First().Write = WriteData;
            HeartrateSources.ElementAt<HeartrateSubscriber>(sourceIndex).Write = WriteData;
            _swapTimer = new Timer(3000);
            _swapTimer.Elapsed += _swapTimer_Elapsed;
            Task.Delay(5000);
            _swapTimer.Start();
        }

        private void _swapTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //Swap the source
            HeartrateSources.ElementAt<HeartrateSubscriber>(sourceIndex).Write = null;
            if (sourceIndex < HeartrateSources.Count - 2)
            {
                sourceIndex++;
                HeartrateSources.ElementAt<HeartrateSubscriber>(sourceIndex).Write = WriteData;
            }
            else
            {
                // All heartrate sources erroneous
                // TODO: initiate alarm to WAS and Mobile
            }
  
        }

        public override void WriteData(HeartrateData data)
        {
            _swapTimer.Stop();
            _swapTimer.Start();
            base.WriteData(data);
        }

        //private void StatusChanged(status status, HeartrateSubscriber sub)
        //{
        //    if (status == unhealthy && sub.Write != null)
        //    {
        //        sub.Write = null;

        //        var newWriter = HeartrateSources.FirstOrDefault(s => s.Status == healthy);
        //        newWriter.Write = WriteData;
        //    }
        //    else if (status == healthy && lowerPrioSourceHasWrite)
        //    {
        //        lowerPrioSource.Write = null;
        //        sub.Write = WriteData;
        //    }


        //}


        public List<HeartrateSubscriber> HeartrateSources { get; } = new List<HeartrateSubscriber>();

        
    }
}

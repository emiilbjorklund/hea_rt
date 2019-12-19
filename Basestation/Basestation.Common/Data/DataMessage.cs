using System;
using System.Collections.Generic;
using System.Text;

namespace Basestation.Common.Data
{
    public abstract class DataMessage
    {
        //Timestamp for time at message generation (unix timestamp, milliseconds)
        public double Timestamp { get; set; }


        //Timestamp for original source (e.g for example heartrate data the timestamp will be the time at which the heartrate was calculated,
        //  and sourcetimestamp would be the timestamp for the original sensor data)
        public double SourceTimestamp { get; set; }


        public double SampleRate { get; set; }
    }
}

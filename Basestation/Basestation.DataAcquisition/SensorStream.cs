using Basestation.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Basestation.DataAcquisition
{
    public abstract class SensorStream<T> : DataStreamer<T>
    {
        public SensorStream(string id)
        {
            Id = id;
        }

        public string Id { get; }
    }
}

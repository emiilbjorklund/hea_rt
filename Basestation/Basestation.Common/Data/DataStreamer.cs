using Basestation.Common.SystemHealth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Basestation.Common.Data
{
    public abstract class DataStreamer<T> : IComponentHealthCheck
    {
        private DateTimeOffset _startTime = DateTimeOffset.MinValue;
        private long _sentMessages = 0;

        public DataStreamer()
        {
            Task.Run(PrintSamplerate);
        }

        private async Task PrintSamplerate()
        {
            while (true)
            {
                await Task.Delay(3000);
                Console.WriteLine(_sentMessages / (DateTimeOffset.Now - _startTime).TotalSeconds);
            }
        }


        private List<Channel<T>> Subscribers { get; } = new List<Channel<T>>();

        public Channel<T> AddSubscriber()
        {
            var channel = Channel.CreateUnbounded<T>();
            Subscribers.Add(channel);
            return channel;
        }

        public void RemoveSubscriber(Channel<T> channel)
        {
            Subscribers.Remove(channel);
        }

        public virtual void WriteData(T data)
        {
            if (_startTime == DateTimeOffset.MinValue)
                _startTime = DateTimeOffset.Now;

            _sentMessages++;

            Parallel.ForEach(Subscribers, (sub) =>
            {
                sub.Writer.TryWrite(data);
            });
        }

        

        bool IComponentHealthCheck.IsComponentHealthy()
        {
            throw new NotImplementedException("Implement to check that data is currently being retrieved");
        }
    }
}

using System;
using System.Threading.Tasks;
using Basestation.Common.Data;
using Basestation.Common.gRPC;
using Grpc.Core;
using LocalHealthEvaluation;
using static LocalHealthEvaluation.LphesHeartrate;

namespace Basestation.MobileAppCommunication.Relay
{
    public class HeartrateSubscriber : DataSubscriber<HeartrateData>
    {
        public Action<HeartrateData> Write { get; set; } 

        public HeartrateSubscriber(string address) : base(address)
        {
        }

        public override async Task ReadStream()
        {
            var client = new LphesHeartrateClient(_channel);
            var replies = client.HeartrateSubscription(new HeartrateRequest() { Id = "test" });
            await foreach (var reply in replies.ResponseStream.ReadAllAsync())
            {
                var data = new HeartrateData() { 
                    Timestamp = reply.UnixTimeStamp,
                    Heartrate = reply.Heartrate
                };

                if (Write != null)
                {
                    Console.WriteLine($"Received new heartrate data: {data.Heartrate}");
                    Write.Invoke(data);
                }
            }
                
        }
    }
}
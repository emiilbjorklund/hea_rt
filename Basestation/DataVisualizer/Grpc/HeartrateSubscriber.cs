using Basestation.Common.Data;
using Basestation.Common.gRPC;
using DataAcquisition;
using Grpc.Core;
using LocalHealthEvaluation;
using System;
using System.Threading.Tasks;
using static LocalHealthEvaluation.LphesHeartrate;

namespace DataVisualizer.Viewmodels
{
    public class HeartrateSubscriber : DataSubscriber<HeartrateData>
    {
        private string _id;
        private Action<HeartrateData> _callback;

        public HeartrateSubscriber(string address, string id, Action<HeartrateData> callback) : base(address)
        {
            _id = id;
            _callback = callback;
        }

        public override async Task ReadStream()
        {
            var client = new LphesHeartrateClient(_channel);
            var replies = client.HeartrateSubscription(new HeartrateRequest() { Id = _id });
            await foreach (var reply in replies.ResponseStream.ReadAllAsync())
                _callback.Invoke(new HeartrateData()
                {
                    Timestamp = reply.UnixTimeStamp,
                    Heartrate = reply.Heartrate
                });
        }
    }
}
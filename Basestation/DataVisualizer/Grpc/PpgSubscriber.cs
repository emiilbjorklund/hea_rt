using Basestation.Common.Data;
using Basestation.Common.gRPC;
using DataAcquisition;
using Grpc.Core;
using System;
using System.Threading.Tasks;
using static DataAcquisition.Sdas;

namespace DataVisualizer.Viewmodels
{
    public class PpgSubscriber : DataSubscriber<PpgData>
    {
        private string _id;
        private Action<PpgData> _callback;

        public PpgSubscriber(string address, string id, Action<PpgData> callback) : base(address)
        {
            _id = id;
            _callback = callback;
        }

        public override async Task ReadStream()
        {
            var client = new SdasClient(_channel);
            var replies = client.PpgSubscription(new PpgSubscriptionRequest() { Id = _id });
            await foreach (var reply in replies.ResponseStream.ReadAllAsync())
                _callback.Invoke(new PpgData()
                {
                    Timestamp = reply.Timestamp,
                    Ppg = reply.Ppg,
                    SampleRate = reply.Samplingrate
                });
        }
    }
}
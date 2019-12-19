using Basestation.Common.Data;
using Basestation.Common.gRPC;
using DataAcquisition;
using Grpc.Core;
using System;
using System.Threading.Tasks;
using static DataAcquisition.Sdas;

namespace DataVisualizer.Viewmodels
{
    public class EcgSubscriber : DataSubscriber<EcgData>
    {
        private string _id;
        private Action<EcgData> _callback;

        public EcgSubscriber(string address, string id, Action<EcgData> callback) : base(address)
        {
            _id = id;
            _callback = callback;
        }

        public override async Task ReadStream()
        {
            var client = new SdasClient(_channel);
            var replies = client.EcgSubscription(new EcgSubscriptionRequest() { Id = _id });
            await foreach (var reply in replies.ResponseStream.ReadAllAsync())
                _callback.Invoke(new EcgData()
                {
                    Timestamp = reply.Timestamp,
                    La_Ra = reply.LaRa,
                    Ll_Ra = reply.LlRa,
                    Vx_Rl = reply.VxRl,
                    SampleRate = reply.Samplingrate
                });
        }
    }
}
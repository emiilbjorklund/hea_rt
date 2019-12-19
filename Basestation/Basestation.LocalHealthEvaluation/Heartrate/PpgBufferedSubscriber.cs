using System.Threading.Tasks;
using Basestation.Common.Data;
using Basestation.Common.gRPC;
using DataAcquisition;
using Grpc.Core;
using static DataAcquisition.Sdas;

namespace Basestation.LocalHealthEvaluation.Heartrate
{
    internal class PpgBufferedSubscriber : BufferedSubscriber<PpgData>
    {
        private string _id;
        public PpgBufferedSubscriber(string address, string id, int bufferSize) : base(address, bufferSize)
        {
            _id = id;
        }

        public override async Task ReadStream()
        {
            var client = new SdasClient(_channel);
            var replies = client.PpgSubscription(new PpgSubscriptionRequest() { Id = _id });
            await foreach (var reply in replies.ResponseStream.ReadAllAsync())
            {
                var data = new PpgData()
                {
                    Timestamp = reply.Timestamp,
                    Ppg = reply.Ppg,
                    SampleRate = reply.Samplingrate
                };
                WriteData(data);
            }
        }
    }
}
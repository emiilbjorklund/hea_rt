using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Basestation.Common.Data;
using Basestation.Common.gRPC;
using DataAcquisition;
using Grpc.Core;
using Grpc.Net.Client;
using static DataAcquisition.Sdas;

namespace Basestation.LocalHealthEvaluation.Heartrate
{
    public class EcgBufferedSubscriber : BufferedSubscriber<EcgData>
    {
        private string _id;
        public EcgBufferedSubscriber(string address, string id, int bufferSize) : base(address, bufferSize)
        {
            _id = id;
        }

        public override async Task ReadStream()
        {
            var client = new SdasClient(_channel);
            var replies = client.EcgSubscription(new EcgSubscriptionRequest() { Id = _id });
            await foreach (var reply in replies.ResponseStream.ReadAllAsync())
            {
                var data = new EcgData()
                {
                    Timestamp = reply.Timestamp,
                    La_Ra = reply.LaRa,
                    Ll_Ra = reply.LlRa,
                    Vx_Rl = reply.VxRl,
                    SampleRate = reply.Samplingrate
                };
                WriteData(data);
            }
        }
    }
}
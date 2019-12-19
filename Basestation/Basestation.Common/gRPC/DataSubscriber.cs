using Basestation.Common.Data;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Basestation.Common.gRPC
{
    public abstract class DataSubscriber<T>
    {
        

        public DataSubscriber(string address)
        {

            _channel = GrpcChannel.ForAddress(address);

            Task.Run(StartStream);
        }

        private async Task StartStream()
        {
            while (true)
            {
                try
                {
                    await ReadStream();
                }
                catch (Exception e)
                {
                    // Implement some warnings or something depending on context
                    await Task.Delay(1000);
                }
            }
        }

        public abstract Task ReadStream();

        public GrpcChannel _channel { get; private set; }

    }
}

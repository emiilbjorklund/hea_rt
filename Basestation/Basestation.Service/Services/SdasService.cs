using Basestation.Common.Data;
using Basestation.DataAcquisition;
using DataAcquisition;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Basestation.Service
{
    public class SdasService : Sdas.SdasBase
    {
        private readonly ILogger<SdasService> _logger;
        private readonly ISensorManager _sensorManager;

        public SdasService(ILogger<SdasService> logger, ISensorManager sensorManager)
        {
            _logger = logger;
            _sensorManager = sensorManager;
        }

        public override async Task EcgSubscription(EcgSubscriptionRequest request, IServerStreamWriter<EcgResponse> responseStream, ServerCallContext context)
        {
            var sensor = _sensorManager.EcgSensors.FirstOrDefault(s => s.Id == request.Id);
            
            Channel<EcgData> channel = null;
            try
            {
                channel = sensor.AddSubscriber();
                await foreach (var msg in channel.Reader.ReadAllAsync(context.CancellationToken))
                {
                    var response = new EcgResponse()
                    {
                        Timestamp = msg.Timestamp,
                        LaRa = ((EcgData)msg).La_Ra,
                        LlRa = ((EcgData)msg).Ll_Ra,
                        VxRl = ((EcgData)msg).Vx_Rl,
                        Samplingrate = msg.SampleRate
                    };
                    await responseStream.WriteAsync(response);
                }
            }
            catch (Exception ex)
            {
                if (channel != null)
                    sensor.RemoveSubscriber(channel);
            }
            return;
        }

        public override async Task PpgSubscription(PpgSubscriptionRequest request, IServerStreamWriter<PpgResponse> responseStream, ServerCallContext context)
        {
            var sensor = _sensorManager.PpgSensors.FirstOrDefault(s => s.Id == request.Id);

            Channel<PpgData> channel = null;
            try
            {
                channel = sensor.AddSubscriber();
                await foreach (var msg in channel.Reader.ReadAllAsync(context.CancellationToken))
                {
                    var response = new PpgResponse()
                    {
                        Timestamp = msg.Timestamp,
                        Ppg = ((PpgData)msg).Ppg,
                        Samplingrate = msg.SampleRate
                    };
                    await responseStream.WriteAsync(response);
                }
            }
            catch (Exception ex)
            {
                if (channel != null)
                    sensor.RemoveSubscriber(channel);
            }
            return;
        }


    }
}

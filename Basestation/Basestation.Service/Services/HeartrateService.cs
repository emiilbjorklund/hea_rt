using Basestation.Common.Data;
using Basestation.DataAcquisition;
using Basestation.LocalHealthEvaluation.Heartrate;
using DataAcquisition;
using Grpc.Core;
using LocalHealthEvaluation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Basestation.Service
{
    public class HeartrateService : LphesHeartrate.LphesHeartrateBase
    {
        private readonly ILogger<SdasService> _logger;
        private readonly IHeartrateEvaluator _heartrateEvaluator;

        public HeartrateService(ILogger<SdasService> logger, IHeartrateEvaluator heartrateEvaluator)
        {
            _logger = logger;
            _heartrateEvaluator = heartrateEvaluator;
        }

        public override async Task HeartrateSubscription(HeartrateRequest request, IServerStreamWriter<HeartrateResponse> responseStream, ServerCallContext context)
        {
            Channel<HeartrateData> channel = null;
            try
            {
                channel = _heartrateEvaluator.AddSubscriber();
                await foreach (var msg in channel.Reader.ReadAllAsync(context.CancellationToken))
                {
                    var response = new HeartrateResponse()
                    {
                        UnixTimeStamp = msg.Timestamp,
                        Heartrate = msg.Heartrate
                    };
                    await responseStream.WriteAsync(response);
                }
            }
            catch (Exception ex)
            {
                if (channel != null)
                    _heartrateEvaluator.RemoveSubscriber(channel);
            }
            return;
        }
    }
}

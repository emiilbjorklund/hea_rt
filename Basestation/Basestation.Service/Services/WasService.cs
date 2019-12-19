using Basestation.WarningsAndAlerts.Warnings;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WarningsAndAlertsGrpc;
using static Basestation.WarningsAndAlerts.Warnings.WarningManager;

namespace Basestation.Service.Services
{
    public class WasService : Was.WasBase
    {
        private IWarningManager _waningManager;
        ILogger<WasService> _logger;
        public WasService(ILogger<WasService> logger, IWarningManager warningManager)
        {
            _waningManager = warningManager;
            _logger = logger;
        }

        public override async Task<WarningResponse> AlarmStatus(WarningRequest request, ServerCallContext context)
        {
            //Execute
            MessageCode code;
            try
            {
                if (!Enum.IsDefined(typeof(MessageCode), request.MessageCode))
                    throw new Exception("Warning failed, message code not defined");
                code = (MessageCode)request.MessageCode;
                _waningManager.TriggerStatusOnDevice(code, request.TriggeringID.ToString());
                return new WarningResponse() { ResponseCode = (int)code };
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{ex}"); // TODO: Remove print
                return new WarningResponse() { ResponseCode = (int)MessageCode.SysFailedExecuteAlert };
            }
        }
    }
}

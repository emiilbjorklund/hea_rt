using System;
using System.Collections.Generic;
using System.Text;
using Basestation.Common.Abstractions;

namespace Basestation.WarningsAndAlerts.Warnings
{
    public class WarningManager : IWarningManager
    {
        private DeviceStatusTrigger trigger; 

        public WarningManager(WarningsAndAlertsCapability capability)
        {
            trigger = new DeviceStatusTrigger();
        }

        // Return true if ok, else false
        public void TriggerStatusOnDevice(MessageCode triggerCode, string triggerID)
        {
            if (triggerCode != MessageCode.SysOk)
            {
                if (trigger.TriggeredServices.ContainsKey(triggerID))
                {
                    // Not allowed for same service to overwrite higher priority alarm
                    if ((int)triggerCode > (int)trigger.TriggeredServices[triggerID])
                        trigger.TriggeredServices[triggerID] = triggerCode;
                }
                else
                    trigger.TriggeredServices[triggerID] = triggerCode;
            }    
            else
                trigger.TriggeredServices.Remove(triggerID);
            
        }
    }

    // According to definitions in was.proto
    // Sorted in order of criticality, where 1 is least critical
    public enum MessageCode
    {
        // Requests according to criticality
        SysOk = 1,
        SysError = 2,
        ArrythmiaWarning = 3,
        SuddenWarning = 4,
        SysFailure = 5,

        // Replies when request code not applicable
        SysFailedExecuteAlert = 999
    }
}

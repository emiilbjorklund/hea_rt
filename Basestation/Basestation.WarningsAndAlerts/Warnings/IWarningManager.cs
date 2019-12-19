using System;
using System.Collections.Generic;
using System.Text;
using static Basestation.WarningsAndAlerts.Warnings.WarningManager;

namespace Basestation.WarningsAndAlerts.Warnings
{
    public interface IWarningManager
    {
        void TriggerStatusOnDevice(MessageCode triggerCode, string triggerID);
        
    }
}

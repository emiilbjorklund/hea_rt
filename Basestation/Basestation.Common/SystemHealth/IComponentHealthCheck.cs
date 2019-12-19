using System;
using System.Collections.Generic;
using System.Text;

namespace Basestation.Common.SystemHealth
{
    public interface IComponentHealthCheck
    {
        //TODO: Implement with new return type for better diagnostics 
        bool IsComponentHealthy();
    }
}

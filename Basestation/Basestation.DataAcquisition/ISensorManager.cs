using Basestation.Common.Data;
using System.Collections.Generic;

namespace Basestation.DataAcquisition
{
    public interface ISensorManager
    {
        List<SensorStream<EcgData>> EcgSensors { get; }
        List<SensorStream<PpgData>> PpgSensors { get; }
    }
}
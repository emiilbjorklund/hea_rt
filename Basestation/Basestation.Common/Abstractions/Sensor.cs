using Basestation.Common.Data;

namespace Basestation.Common.Abstractions
{
    public class Sensor
    {
        public Sensor(string id, SensorType type)
        {
            Id = id;
            Type = type;
        }

        public string Id { get; private set; }
        public SensorType Type { get; private set; }
    }

    public enum SensorType
    {
        EcgTestData,
        PpgTestData,
        ShimmerEcg,
        ShimmerPpg
    }
}
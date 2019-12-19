using System.Collections.Generic;

namespace Basestation.Common.Abstractions
{
    public class HealthEvaluation
    {
        public HealthEvaluation()
        {

        }

        public List<Sensor> Sources { get; private set; } = new List<Sensor>();
        public List<Service> AlertServices { get; private set; } = new List<Service>();
    }
}
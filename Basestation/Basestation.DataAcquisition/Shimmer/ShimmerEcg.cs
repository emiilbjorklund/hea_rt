using ShimmerAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAcquisition;
using Basestation.Common.Data;

namespace Basestation.DataAcquisition.Shimmer
{
    public class ShimmerEcg : SensorStream<EcgData>
    {
        private Shimmer _shimmer;

        public ShimmerEcg(string id) : base(id)
        {
            _shimmer = new Shimmer(
               "ECG",
               id, //TODO: Find port via bluetooth stack instead of specified in yml
               "ecg",
               256,
               ((int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_EXG1_24BIT | (int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_EXG2_24BIT),
               4,
               new List<string>(new string[] { "INTERNAL_ADC_A13" }));
            _shimmer.NewEcgResponse += _shimmer_NewEcgResponse;
        }

        private void _shimmer_NewEcgResponse(object sender, NewEcgResponseEventArgs e)
        {
            WriteData(e.Ecg);
        }
    }
}

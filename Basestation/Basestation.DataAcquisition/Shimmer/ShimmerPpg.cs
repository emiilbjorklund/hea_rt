using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using Basestation.Common.Data;
using DataAcquisition;
using ShimmerAPI;

namespace Basestation.DataAcquisition.Shimmer
{
    public class ShimmerPpg : SensorStream<PpgData>
    {
        private Shimmer _shimmer;

        public ShimmerPpg(string macAddress) : base(macAddress)
        {
            _shimmer = new Shimmer(
               "testID",
               "COM3",
               "ppg",
               256,
               ((int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_GSR | (int)ShimmerBluetooth.SensorBitmapShimmer3.SENSOR_INT_A13),
               4,
               new List<string>(new string[] { "INTERNAL_ADC_A13" }));
            _shimmer.NewPpgResponse += _shimmer_NewPpgResponse;
        }

        private void _shimmer_NewPpgResponse(object sender, NewPpgResponseEventArgs e)
        {
            WriteData(e.Ppg);
        }
    }
}

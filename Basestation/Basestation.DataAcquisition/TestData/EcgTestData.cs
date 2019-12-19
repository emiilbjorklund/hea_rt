using Basestation.Common.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Basestation.DataAcquisition.TestData
{
    public class EcgTestData : SensorStream<EcgData>
    {
        private int ecgIndex = 0;
        private EcgData[] ecgData;

        public EcgTestData(string id) : base(id)
        {
            EcgInit();

            var ecgtimer = new Timer();
            ecgtimer.Interval = 250;
            ecgtimer.Elapsed += EcgTimer;
            ecgtimer.Start();
        }

        private void EcgTimer(object sender, ElapsedEventArgs e)
        {
            //Every 250 msec interval, write 64 msgs to channels

            for (int i = 0; i < 64; i++)
            {
                WriteData(ecgData[ecgIndex]);

                ecgIndex++;
                if (ecgIndex >= ecgData.Length)
                    ecgIndex = 0;
            }

            //Console.WriteLine($"{Id}: Wrote 128 msgs");
        }

        public void EcgInit()
        {
            var format = new NumberFormatInfo();
            format.NegativeSign = "-";


            var path = Path.Combine("TestData", "ecgsample.csv");
            if (!File.Exists(path))
                path = Path.Combine("bin", "Debug", "netcoreapp3.0", "TestData", "ecgsample.csv");

            var lines = File.ReadAllLines(path).ToArray();
            ecgData = new EcgData[lines.Count()];
            for (int i = 0; i < lines.Count(); i++)
            {
                var parts = lines[i].Split(';', StringSplitOptions.RemoveEmptyEntries);
                var msg = new EcgData();


                msg.Timestamp = double.Parse(parts[0], format);
                msg.SourceTimestamp = msg.Timestamp;
                msg.Ll_Ra = double.Parse(parts[1], format);
                msg.La_Ra = double.Parse(parts[2], format);
                msg.Vx_Rl = double.Parse(parts[3], format);
                msg.SampleRate = 256;

                ecgData[i] = msg;
            }
        }
    }
}

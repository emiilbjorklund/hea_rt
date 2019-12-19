using Basestation.Common.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;

namespace Basestation.DataAcquisition.TestData
{
    public class PpgTestData : SensorStream<PpgData>
    {
        private int ppgIndex = 0;
        private PpgData[] ppgData;

        public PpgTestData(string id) : base(id)
        {
            PpgInit();

            var ecgtimer = new Timer();
            ecgtimer.Interval = 250;
            ecgtimer.Elapsed += PpgTimer;
            ecgtimer.Start();
        }

        private void PpgTimer(object sender, ElapsedEventArgs e)
        {
            //Every 250 msec interval, write 64 msgs to channels

            for (int i = 0; i < 64; i++)
            {
                WriteData(ppgData[ppgIndex]);

                ppgIndex++;
                if (ppgIndex >= ppgData.Length)
                    ppgIndex = 0;
            }

            //Console.WriteLine($"{Id}: Wrote 128 msgs");
        }

        public void PpgInit()
        {
            var format = new NumberFormatInfo();
            format.NegativeSign = "-";


            var path = Path.Combine("TestData", "ppgsample.csv");
            if (!File.Exists(path))
                path = Path.Combine("bin", "Debug", "netcoreapp3.0", "TestData", "ppgsample.csv");

            var lines = File.ReadAllLines(path).ToArray();
            ppgData = new PpgData[lines.Count()];
            for (int i = 0; i < lines.Count(); i++)
            {
                var parts = lines[i].Split(';', StringSplitOptions.RemoveEmptyEntries);
                var msg = new PpgData();


                msg.Timestamp = double.Parse(parts[0], format);
                msg.SourceTimestamp = msg.Timestamp;
                msg.Ppg = double.Parse(parts[1], format);
                msg.SampleRate = 256;

                ppgData[i] = msg;
            }
        }

    }
}

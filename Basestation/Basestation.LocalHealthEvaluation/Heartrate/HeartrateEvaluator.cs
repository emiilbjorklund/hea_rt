using Basestation.Common.Abstractions;
using Basestation.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace Basestation.LocalHealthEvaluation.Heartrate
{
    public class HeartrateEvaluator : DataStreamer<HeartrateData>, IHeartrateEvaluator
    {
        private Timer _calculationTimer;

        public HeartrateEvaluator(HeartrateEvaluationCapability capability, int samplerate = 1)
        {
            //TODO: Loop and add source subscriptions
            foreach (var ecg in capability.EcgSources)
                ecgSubscribers.Add(new EcgBufferedSubscriber(capability.GetAddress(ecg), ecg, 256 * 4));

            foreach (var ppg in capability.PpgSources)
                ppgSubscribers.Add(new PpgBufferedSubscriber(capability.GetAddress(ppg), ppg, 256 * 16));

            if (ecgSubscribers.Count > 0)
                ecgSubscribers.First().Buffer = EcgBuffer;
            if (ppgSubscribers.Count > 0)
                ppgSubscribers.First().Buffer = PpgBuffer;

            //TODO: Loop and add alert service targets

            //TODO: Start evaluation

            _calculationTimer = new Timer(1000);
            _calculationTimer.Elapsed += _calculationTimer_Elapsed;
            _calculationTimer.Start();

        }

        private void _calculationTimer_Elapsed(object sender, ElapsedEventArgs e)
        {

            Console.WriteLine($"Ecg buffer size: {EcgBuffer.Count}");
            Console.WriteLine($"Ppg buffer size: {PpgBuffer.Count}");
            if (EcgBuffer.Count > 0)
            {
                Console.WriteLine($"Timestamp: {EcgBuffer.Last().Timestamp}");
                Console.WriteLine($"LA-RA: {EcgBuffer.Last().La_Ra}");
                Console.WriteLine($"LL-LA: {EcgBuffer.Last().Ll_La}");
                Console.WriteLine($"LL-RA: {EcgBuffer.Last().Ll_Ra}");
                Console.WriteLine($"LL-RA: {EcgBuffer.Last().SampleRate}");
            }

            var hrEcg = HeartrateCalculations.CalculateEcgHeartrate(EcgBuffer, out double newestEcgTimestamp);
            var hrPpg = HeartrateCalculations.CalculatePpgHeartrate(PpgBuffer, out double newestPpgTimestamp);

            HeartrateData data;

            if (ecgSubscribers.Count < 1)
            {
                data = new HeartrateData()
                {
                    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    SourceTimestamp = newestPpgTimestamp,
                    Heartrate = hrPpg,
                    SampleRate = 1
                };
            }
            else
            {
                data = new HeartrateData()
                {
                    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    SourceTimestamp = newestEcgTimestamp,
                    Heartrate = hrEcg,
                    SampleRate = 1
                };
            }
            

            WriteData(data);
            Console.WriteLine($"ECG HR: {hrEcg}");
            Console.WriteLine($"PPG HR: {hrPpg}");
        }

        private List<EcgBufferedSubscriber> ecgSubscribers = new List<EcgBufferedSubscriber>();
        private List<PpgBufferedSubscriber> ppgSubscribers = new List<PpgBufferedSubscriber>();

        public List<EcgData> EcgBuffer { get; } = new List<EcgData>();
        public List<PpgData> PpgBuffer { get; } = new List<PpgData>();

    }
}

using Basestation.Common.Data;
using MathNet.Filtering;
using MathNet.Numerics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ShimmerLibrary;
using System.Collections.Concurrent;
using ShimmerAPI;

namespace Basestation.LocalHealthEvaluation.Heartrate
{
    public static class HeartrateCalculations
    {
        private static Filter _LPFilter = new Filter(Filter.LOW_PASS, 256, new double[] { 5.0 });
        private static Filter _HPFilter = new Filter(Filter.HIGH_PASS, 256, new double[] { 0.5 });
        private static PPGToHRAlgorithm PPGtoHeartRateCalculation = null;
        private static double _lastppg = 0;

        private static int csvIndex = 1;
        private static readonly double RMIN = 0.7;
        private static readonly double RINTERVALMIN = 200;
        private static double lastPeakR;

        public static double CalculatePpgHeartrate(List<PpgData> ppgBuffer, out double timestamp)
        {
            timestamp = -1;

            if (ppgBuffer.Count < 256 * 3)
                return 0;

            

            List<PpgData> bufferCopy;
            lock (ppgBuffer)
            {
                bufferCopy = ppgBuffer.ToList();
            }

            if (PPGtoHeartRateCalculation == null)
                PPGtoHeartRateCalculation = new PPGToHRAlgorithm(bufferCopy.Last().SampleRate);

            timestamp = bufferCopy.Last().Timestamp;

            var indexOfFirstNew = bufferCopy.IndexOf(bufferCopy.First(x => x.Timestamp > _lastppg));
            Console.WriteLine(indexOfFirstNew);
            _lastppg = bufferCopy.Last().Timestamp;
            Console.WriteLine(_lastppg);

            var hr = PPGtoHeartRateCalculation.ppgToHrConversion(bufferCopy.Skip(indexOfFirstNew).Select(p => _HPFilter.filterData(_LPFilter.filterData(p.Ppg))).ToArray(), bufferCopy.Skip(indexOfFirstNew).Select(p => p.Timestamp).ToArray());

            var hres = hr.LastOrDefault(x => x != -1);
            if (hres == 0)
                return -1;
            else
                return hres;

            //var samplingrate = bufferCopy.First().SampleRate;
            //PPGtoHeartRateCalculation = new PPGToHRAlgorithm(samplingrate);
            //var hr = PPGtoHeartRateCalculation.ppgToHrConversion(bufferCopy.Select(p => p.Ppg).ToArray(), bufferCopy.Select(p => p.Timestamp).ToArray());


            //double hr2 = 0;
            //foreach (var p in bufferCopy)
            //    hr2 = PPGtoHeartRateCalculation.ppgToHrConversion(p.Ppg, p.Timestamp);
            //PrintCsv("unknown", bufferCopy);

            //if (heartRateECG < 50)
            //    PrintCsv("bad", bufferCopy);
            //else if (heartRateECG > 120)
            //    PrintCsv("bad", bufferCopy);

            return hr.Last();
        }


        public static double CalculateEcgHeartrate(List<EcgData> ecgBuffer, out double timestamp)
        {
            timestamp = -1;

            if (ecgBuffer.Count < 256*3)
                return -1;


            List<EcgData> bufferCopy;
            lock (ecgBuffer)
            {
                bufferCopy = ecgBuffer.ToList();
            }

            timestamp = bufferCopy.Last().Timestamp;
            var samplingrate = bufferCopy.First().SampleRate;
            var datapoints = ConvertTimeDataPair(bufferCopy);
            var filteredData = NotchFilter(datapoints, samplingrate);
            var detrend = DetrendCurve(filteredData);
            var rPeak = FindRpeak(detrend);

            var heartRateECG = CalculateEcgHr(rPeak);


            //PrintCsv("unknown", bufferCopy);

            //if (heartRateECG < 50)
            //    PrintCsv("bad", bufferCopy);
            //else if (heartRateECG > 120)
            //    PrintCsv("bad", bufferCopy);

            return heartRateECG;
        }

        private static void PrintCsv(string result, List<EcgData> buffer)
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "EcgTest");
            Directory.CreateDirectory(path);

            var lines = new List<string>();
            foreach (var data in buffer)
            {
                var line = $"{data.Timestamp};{data.Ll_Ra};{data.La_Ra};{data.Vx_Rl};{data.Ll_La}";
                lines.Add(line);
            }

            File.WriteAllLines(Path.Combine(path, $"{csvIndex++} - {result}.csv"), lines.ToArray());
        }

        private static List<TimeDataPair> ConvertTimeDataPair(List<EcgData> bufferCopy)
        {
            List<TimeDataPair> result = new List<TimeDataPair>();
            foreach (var data in bufferCopy)
                result.Add(new TimeDataPair(data.Timestamp, data.La_Ra + data.Ll_Ra + data.Ll_La));
            return result;
        }

        private static List<TimeDataPair> NotchFilter(List<TimeDataPair> data, double samplingrate)
        {
            var bandstop = OnlineFilter.CreateBandstop(ImpulseResponse.Finite, samplingrate, 45, 55, 7);
            List<TimeDataPair> returnData = new List<TimeDataPair>();

            foreach (var item in data)
            {
                var filteredData = bandstop.ProcessSample(item.DataPoint);
                returnData.Add(new TimeDataPair(item.TimeStamp, filteredData));
            }

            return returnData;
        }

        private static List<TimeDataPair> DetrendCurve(List<TimeDataPair> data)
        {
            List<TimeDataPair> dataDetrend = new List<TimeDataPair>();
            double[] scaledX = new double[data.Count];
            double[] calcPolyZero = new double[data.Count];

            var x = Generate.LinearRange(1, 1, data.Count);

            double mu = x.Average(); // mean value
            double sumsqr = x.Select(val => (val - mu) * (val - mu)).Sum(); // Standard deviation 
            double std = Math.Sqrt(sumsqr / data.Count); // Standard deviation 

            for (int i = 0; i < data.Count; i++)
            {
                scaledX[i] = (x[i] - mu) / std;  // Centering and scaling 
            }
            var polyfit = Polynomial.Fit(scaledX, data.Select(timeDataPair => timeDataPair.DataPoint).ToArray(), 6); // Scaled polynomial function 
            var poly = polyfit.Coefficients; // Extract polynomial coefficients 

            for (int i = 0; i < data.Count; i++) // Calculate deviation from zero  
            {
                var calc1 = Math.Pow((x[i] - mu) / std, 1);
                var calc2 = Math.Pow((x[i] - mu) / std, 2);
                var calc3 = Math.Pow((x[i] - mu) / std, 3);
                var calc4 = Math.Pow((x[i] - mu) / std, 4);
                var calc5 = Math.Pow((x[i] - mu) / std, 5);
                var calc6 = Math.Pow((x[i] - mu) / std, 6);

                calcPolyZero[i] = (poly[0]) + (calc1 * poly[1]) + (calc2 * poly[2]) + (calc3 * poly[3]) + (calc4 * poly[4]) + (calc5 * poly[5]) + (calc6 * poly[6]);

            }

            // Remove deviation from filtered signal
            for (int i = 0; i < data.Count; i++)
            {
                data[i].DataPoint = data[i].DataPoint - calcPolyZero[i];
            }

            return data;
        }

        private static List<TimeDataPair> FindRpeak(List<TimeDataPair> data)
        {
            List<TimeDataPair> Rpeak = new List<TimeDataPair>();
            for (int i = 0; i < data.Count; i++)
            {

                if ((data[i].DataPoint >= RMIN) && (data.Count - i > 30) && (data[i + 30].TimeStamp /*- lastPeakR*/ > RINTERVALMIN))
                {
                    var subList = data.GetRange(i, 30);
                    var max = subList.Max(m => m.DataPoint);
                    var pos = subList.FindIndex(i => i.DataPoint == max);
                    lastPeakR = data[i + pos].TimeStamp;
                    Rpeak.Add(new TimeDataPair(data[i + pos].TimeStamp, data[i + pos].DataPoint, i + pos));

                    i = i + 30;
                }
            }

            return Rpeak;
        }

        private static double CalculateEcgHr(List<TimeDataPair> data)
        {
            List<TimeSpan> intervalRR = new List<TimeSpan>();

            if (data.Count <= 1)
            {
                int hr = 0;

                return hr;
            }

            for (int i = 0; i < data.Count; i++)
            {
                if ((i + 1) < data.Count)
                {
                    intervalRR.Add(UnixTimeToDateTime(data[i + 1].TimeStamp).Subtract(UnixTimeToDateTime(data[i].TimeStamp)));
                }
            }

            var heartRateECG = (60 / intervalRR.Average(val => val.TotalSeconds));

            return heartRateECG;
        }

        public static DateTime UnixTimeToDateTime(double unixtime)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixtime).ToLocalTime();
            return dtDateTime;
        }
    }

    public class TimeDataPair
    {
        private double _timeStamp;
        private double _data;
        private int _index;


        public TimeDataPair(double timeStamp, double dataPoint, int index)
        {
            TimeStamp = timeStamp;
            DataPoint = dataPoint;
            Index = index;
        }

        public TimeDataPair(double timeStamp, double dataPoint)
        {
            TimeStamp = timeStamp;
            DataPoint = dataPoint;

        }

        public double TimeStamp { get => _timeStamp; set => _timeStamp = value; }
        public double DataPoint { get => _data; set => _data = value; }
        public int Index { get => _index; set => _index = value; }
    }
}

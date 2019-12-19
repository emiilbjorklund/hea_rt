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
using DataVisualizer.Models;

namespace DataVisualizer.EcgProcessing
{
    public static class HeartrateCalculations
    {
        //private static PPGToHRAlgorithm PPGtoHeartRateCalculation;
        private static int csvIndex = 1;
        private static readonly double RMIN = 0.7;
        private static readonly double RINTERVALMIN = 200;
        private static double lastPeakR;

        //public static double CalculatePpgHeartrate(List<PpgData> ppgBuffer)
        //{
        //    if (ppgBuffer.Count < 1)
        //    {
        //        Console.WriteLine(ppgBuffer.Count);
        //        return 0;
        //    }

        //    PpgData dataPoint;
        //    lock (ppgBuffer)
        //    {
        //        dataPoint = ppgBuffer.Last();
        //    }

        //    return PPGtoHeartRateCalculation.ppgToHrConversion(dataPoint.Ppg, dataPoint.Timestamp);
        //}


        //public static double CalculateEcgHeartrate(List<EcgData> ecgBuffer, out double timestamp)
        //{
        //    var filteredData = NotchFilter(datapoints, samplingrate);
        //    var detrend = DetrendCurve(filteredData);
        //    var rPeak = FindRpeak(detrend);

        //    var heartRateECG = CalculateEcgHr(rPeak);


        //    PrintCsv("unknown", bufferCopy);

        //    //if (heartRateECG < 50)
        //    //    PrintCsv("bad", bufferCopy);
        //    //else if (heartRateECG > 120)
        //    //    PrintCsv("bad", bufferCopy);

        //    return heartRateECG;
        //}


        private static List<TimeDataPair> ConvertTimeDataPair(List<EcgData> bufferCopy)
        {
            List<TimeDataPair> result = new List<TimeDataPair>();
            foreach (var data in bufferCopy)
                result.Add(new TimeDataPair(data.Timestamp, data.La_Ra + data.Ll_Ra + data.Ll_La));
            return result;
        }

        private static int i = 0;
        private static OnlineFilter BandstopFilter = null;

        public static DataPoint NotchFilterSingle(DataPoint data, double samplingrate)
        {
            if (BandstopFilter == null)
                BandstopFilter = OnlineFilter.CreateBandstop(ImpulseResponse.Finite, samplingrate, 45, 55, 7);

            var filteredData = BandstopFilter.ProcessSample(data.Value);


            return new DataPoint() { Timestamp = data.Timestamp, Value = filteredData };
        }

        public static List<DataPoint> DetrendCurve(List<DataPoint> filtered)
        {
            List<DataPoint> data = new List<DataPoint>();
            foreach (var d in filtered)
                data.Add(new DataPoint() { Timestamp = d.Timestamp, Value = d.Value });


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
            var polyfit = Polynomial.Fit(scaledX, data.Select(timeDataPair => timeDataPair.Value).ToArray(), 6); // Scaled polynomial function 
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
                data[i].Value = data[i].Value - calcPolyZero[i];
            }

            return data;
        }

        public static List<DataPoint> FindRpeak(List<DataPoint> data)
        {
            List<DataPoint> Rpeak = new List<DataPoint>();
            for (int i = 0; i < data.Count; i++)
            {

                if ((data[i].Value >= RMIN) && (data.Count - i > 30) && (data[i + 30].Timestamp /*- lastPeakR*/ > RINTERVALMIN))
                {
                    var subList = data.GetRange(i, 30);
                    var max = subList.Max(m => m.Value);
                    var pos = subList.FindIndex(i => i.Value == max);
                    lastPeakR = data[i + pos].Timestamp;
                    Rpeak.Add(new DataPoint(/*data[i + pos].Timestamp, data[i + pos].Value, i + pos*/) { Timestamp = data[i + pos].Timestamp, Value = data[i + pos].Value });

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

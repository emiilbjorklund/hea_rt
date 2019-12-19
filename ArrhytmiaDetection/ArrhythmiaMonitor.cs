using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using MathNet.Filtering;
using MessageLibrary;
using MathNet.Numerics;
using System.Linq;
using Extreme.Mathematics.SignalProcessing;

namespace HealthMonitor
{
    class ArrhythmiaMonitor
    {
        OnlineFilter bandstop;

        private static double RMIN = 0.9;           //mV
        private static double RINTERVALMIN = 200;   //ms

        private double HR;
        private double PR;
        private double QRS;
        private double PP;
        private double HealthCond;
        private double lastPeakR;
        private double freq;
        private int sampleSize;

        List<timeDataPairIndex> Ppeak;
        List<timeDataPairIndex> Qpeak;
        List<timeDataPairIndex> Rpeak;
        List<timeDataPairIndex> Speak;

        public ArrhythmiaMonitor(double Freq, int SampleSize)
        {
            freq = Freq;
            sampleSize = SampleSize;

            bandstop = OnlineFilter.CreateBandstop(ImpulseResponse.Finite, freq, 45, 55, 7);

            Ppeak = new List<timeDataPairIndex>();
            Qpeak = new List<timeDataPairIndex>();
            Rpeak = new List<timeDataPairIndex>();
            Speak = new List<timeDataPairIndex>();
        }

        private void clear()
        {
            lastPeakR = 0;
            HR = 0;
            PR = 0;
            QRS = 0;
            PP = 0;
            HealthCond = 0;

        Ppeak.Clear();
            Qpeak.Clear();
            Rpeak.Clear();
            Speak.Clear();
        }


        public void ecgToArConversion(List<timeDataPair> data)
        {
            clear();

            var filteredData = notchFilter(data);
            var detrend = detrendCurve(filteredData);
            findRpeak(detrend);
            findPQSpeak(detrend);

            calculateAR();

           //For Evaluation
           writeDataToFile(detrend);
        }

        private void writeDataToFile(List<timeDataPair> data)
        {
            using (TextWriter sw = new StreamWriter(@"Arrhytmia.csv"))
            {
                for (int i = 0; i < sampleSize - 1; i++)
                {
                    sw.WriteLine("{0}", data[i].DataPoint.ToString("R"));
                }

                Console.WriteLine("\nP");
                foreach (var item in Ppeak)
                {
                    Console.WriteLine(item.Index);
                }
                Console.WriteLine("\nQ");
                foreach (var item in Qpeak)
                {
                    Console.WriteLine(item.Index);
                }
                Console.WriteLine("\nR");
                foreach (var item in Rpeak)
                {
                    Console.WriteLine(item.Index);
                }
                Console.WriteLine("\nS");
                foreach (var item in Speak)
                {
                    Console.WriteLine(item.Index);
                }

            }

            Console.WriteLine("DONE");
        }

        private List<timeDataPair> notchFilter(List<timeDataPair> data)
        {
            List<timeDataPair> returnData = new List<timeDataPair>();

            foreach (var item in data)
            {
                var filteredData = bandstop.ProcessSample(item.DataPoint);
                returnData.Add(new timeDataPair(item.TimeStamp, filteredData));
            }

            return returnData;
        }

        private List<timeDataPair> detrendCurve(List<timeDataPair> data)
        {
            List<timeDataPair> dataDetrend = new List<timeDataPair>();
            double[] scaledX = new double[data.Count];
            double[] calcPolyZero = new double[data.Count];

            var x = Generate.LinearRange(1, 1, data.Count);

            double mu = x.Average(); // Mean value
            double sumsqr = x.Select(val => (val - mu) * (val - mu)).Sum(); // Standard deviation 
            double std = Math.Sqrt(sumsqr / data.Count); // Standard deviation 

            for (int i = 0; i < data.Count; i++)
            {
                scaledX[i] = (x[i] - mu) / std;  // Centering and scaling 
            }
            var polyfit = Polynomial.Fit(scaledX, data.Select(timeDataPair => timeDataPair.DataPoint).ToArray(), 6); // Scaled polynomial function 
            var poly = polyfit.Coefficients; // Extract polynomial coefficients 

            // Calculate deviation from zero 
            for (int i = 0; i < data.Count; i++)  
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

        private void findRpeak(List<timeDataPair> data)
        {

            for (int i = 0; i < sampleSize; i++)
            {
                if ((data[i].DataPoint >= RMIN) && (sampleSize - i > 30) && (data[i + 30].TimeStamp - lastPeakR > RINTERVALMIN))
                {
                    var subList = data.GetRange(i, 30);
                    var max = subList.Max(m => m.DataPoint);
                    var pos = subList.FindIndex(i => i.DataPoint == max);
                    lastPeakR = data[i + pos].TimeStamp;
                    Rpeak.Add(new timeDataPairIndex(data[i + pos].TimeStamp, data[i + pos].DataPoint, i + pos));

                    i = i + 30;
                }
            }
        }

        private void findPQSpeak(List<timeDataPair> data)
        {
            foreach (var item in Rpeak)
            {
                if (item.Index <= 30) // Minimum samples for detection
                {
                    var subListQ = data.GetRange(0, 30);
                    var minQ = subListQ.Min(m => m.DataPoint);
                    var posQ = subListQ.FindIndex(i => i.DataPoint == minQ);
                    Qpeak.Add(new timeDataPairIndex(data[item.Index + posQ].TimeStamp, data[item.Index + posQ].DataPoint, item.Index + posQ));

                }
                else
                {
                    var subListQ = data.GetRange(item.Index - 30, 30);
                    var minQ = subListQ.Min(m => m.DataPoint);
                    var posQ = subListQ.FindIndex(i => i.DataPoint == minQ);
                    Qpeak.Add(new timeDataPairIndex(data[item.Index - 30 + posQ].TimeStamp, data[item.Index - 30 + posQ].DataPoint, item.Index - 30 + posQ));

                    if (item.Index - 30 + posQ <= 70) // Samples checked for interval
                    {
                        var subListP = data.GetRange(0, 70);
                        var maxP = subListP.Max(m => m.DataPoint);
                        var posP = subListP.FindIndex(i => i.DataPoint == maxP);
                        Ppeak.Add(new timeDataPairIndex(data[item.Index + posP].TimeStamp, data[item.Index + posP].DataPoint, item.Index + posP));

                    }
                    else
                    {
                        var subListP = data.GetRange(item.Index - 30 + posQ - 70, 70);
                        var minP = subListQ.Min(m => m.DataPoint);
                        var posP = subListQ.FindIndex(i => i.DataPoint == minP);
                        Ppeak.Add(new timeDataPairIndex(data[item.Index - 30 + posQ - 70 + posP].TimeStamp, data[item.Index - 30 + posQ - 70 + posP].DataPoint, item.Index - 30 + posQ - 70 + posP));
                    }
                }

                if (data.Count - item.Index < 100) // Maximum time interval 
                {
                    var subListS = data.GetRange(item.Index, data.Count - item.Index);
                    var minS = subListS.Min(m => m.DataPoint);
                    var posS = subListS.FindIndex(i => i.DataPoint == minS);
                    Speak.Add(new timeDataPairIndex(data[item.Index + posS].TimeStamp, data[item.Index + posS].DataPoint, item.Index + posS));
                }
                else
                {
                    var subListS = data.GetRange(item.Index, 100);
                    var minS = subListS.Min(m => m.DataPoint);
                    var posS = subListS.FindIndex(i => i.DataPoint == minS);
                    Speak.Add(new timeDataPairIndex(data[item.Index + posS].TimeStamp, data[item.Index + posS].DataPoint, item.Index + posS));
                }
            }
        }

        private void calculateAR()
        {
            List<TimeSpan> intervalRR = new List<TimeSpan>();
            List<TimeSpan> intervalPP = new List<TimeSpan>();
            List<TimeSpan> intervalPR = new List<TimeSpan>();
            List<TimeSpan> intervalQRS = new List<TimeSpan>();

            for (int i = 0; i < Rpeak.Count; i++)
            {
                if ((i+1)< Rpeak.Count)
                {
                    intervalRR.Add(UnixTimeToDateTime(Rpeak[i + 1].TimeStamp).Subtract(UnixTimeToDateTime(Rpeak[i].TimeStamp)));
                    intervalPP.Add(UnixTimeToDateTime(Ppeak[i + 1].TimeStamp).Subtract(UnixTimeToDateTime(Ppeak[i].TimeStamp)));
                }
                intervalPR.Add(UnixTimeToDateTime(Rpeak[i].TimeStamp).Subtract(UnixTimeToDateTime(Ppeak[i].TimeStamp)));
                intervalQRS.Add(UnixTimeToDateTime(Speak[i].TimeStamp).Subtract(UnixTimeToDateTime(Qpeak[i].TimeStamp)));
            }

            HR = (60 / intervalRR.Average(val => val.TotalSeconds)); // Heart rate calculation
            PR = intervalPR.Average(val => val.TotalSeconds); // PR interval calculation 
            QRS = intervalQRS.Average(val => val.TotalSeconds); // QRS interval calculation

            if ((HR > 50 && HR < 100) && (PR > 0.12 && PR < 0.2) && (QRS > 0.06 && QRS < 0.12)) // Check normal health condition 
            {
                
                for (int i = 0; i < intervalPP.Count - 1; i++)
                    if ((1 - (intervalPP[i].TotalSeconds / intervalPP[i + 1].TotalSeconds)) > 0.10)
                    {
                        HealthCond = 1; // Arrythmia detected
                    }
                    else
                    {
                        HealthCond = 0;  // No arrythmia detected 
                    }
            }
            else
            {
                HealthCond = 1; // Arrythmia detected
            }

            Console.WriteLine(HR);
            Console.WriteLine(HealthCond);
            Console.WriteLine(PR);
            foreach (var item in intervalPP)
            {
                Console.WriteLine(item.TotalSeconds);
            }

            Console.WriteLine(QRS);
        }

        public DateTime UnixTimeToDateTime(double unixtime)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixtime).ToLocalTime();
            return dtDateTime;
        }
    }
        
    public class timeDataPairIndex
    {
        private double _timeStamp;
        private double _data;
        private int _index;


        public timeDataPairIndex(double timeStamp, double dataPoint, int index)
        {
            TimeStamp = timeStamp;
            DataPoint = dataPoint;
            Index = index;
        }

        public double TimeStamp { get => _timeStamp; set => _timeStamp = value; }
        public double DataPoint { get => _data; set => _data = value; }
        public int Index { get => _index; set => _index = value; }
    }
}
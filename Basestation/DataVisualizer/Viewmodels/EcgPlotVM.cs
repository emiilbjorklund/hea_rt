using Basestation.Common.Data;
using Basestation.Common.gRPC;
using DataVisualizer.EcgProcessing;
using DataVisualizer.Models;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DataVisualizer.Viewmodels
{
    public class EcgPlotVM : BaseVM
    {
        //View bindings
        private bool m_showLlRa;
        private bool m_showLaRa;
        private bool m_showVxRl;
        private bool m_showLlLa;

        private bool m_showMerged;
        private bool m_showFiltered;
        private bool m_showDetrend;
        private bool m_showHeartrate;

        //Chart x axis
        private double _axisMax = 10;
        private double _axisMin = 0;

        private EcgSubscriber m_subscriber;
        //public List<EcgData> Buffer { get; } = new List<EcgData>();

        public EcgPlotVM(string address, string targetId)
        {
            m_subscriber = new EcgSubscriber(address, targetId, OnNewData);
            //m_subscriber.Buffer = Buffer;

            var mapper = Mappers.Xy<DataPoint>()
                .X(model => model.Timestamp)
                .Y(model => model.Value);

            Charting.For<DataPoint>(mapper);


            ///Series.Add(new LineSeries());

            //Task.Run(Test);
        }

        public void OnNewData(EcgData data)
        {
            LaRa.Add(new DataPoint { Value = data.La_Ra, Timestamp = data.Timestamp });
            while (LaRa.Count > 256 * 4)
                LaRa.RemoveAt(0);

            LlRa.Add(new DataPoint { Value = data.Ll_Ra, Timestamp = data.Timestamp });
            while (LlRa.Count > 256 * 4)
                LlRa.RemoveAt(0);

            VxRl.Add(new DataPoint { Value = data.Vx_Rl, Timestamp = data.Timestamp });
            while (VxRl.Count > 256 * 4)
                VxRl.RemoveAt(0);

            LlLa.Add(new DataPoint { Value = data.Ll_La, Timestamp = data.Timestamp });
            while (LlLa.Count > 256 * 4)
                LlLa.RemoveAt(0);

            Merged.Add(new DataPoint { Value = data.La_Ra + data.Ll_Ra + data.Ll_La, Timestamp = data.Timestamp });
            while (Merged.Count > 256 * 4)
                Merged.RemoveAt(0);

            Filtered.Add(HeartrateCalculations.NotchFilterSingle(Merged.Last(), data.SampleRate));
            while (Filtered.Count > 256 * 4)
                Filtered.RemoveAt(0);

            var detrend = HeartrateCalculations.DetrendCurve(Filtered.ToList());
            Detrend.Clear();
            Detrend.AddRange(detrend);

            var heartbeats = HeartrateCalculations.FindRpeak(detrend);
            Heartrate.Clear();
            Heartrate.AddRange(heartbeats);


            if (LlLa.Count > 1)
                SetAxisLimits(LlLa.First().Timestamp, LlLa.Last().Timestamp);

        }

        //private async Task Test()
        //{
        //    var t = 0;
        //    var v = 0;
        //    while (true)
        //    {
        //        //App.Current.Dispatcher.Invoke(() => LlLa.Add(new DataPoint() { Timestamp = t++, Value = v++ }));

        //        LlLa.Add(new DataPoint() { Timestamp = t++, Value = v++ });
        //        while (LlLa.Count > 256 * 4)
        //            LlLa.RemoveAt(0);


        //        if (LlLa.Count > 1)
        //            SetAxisLimits(LlLa.First().Timestamp, LlLa.Last().Timestamp);
        //        await Task.Delay(33);
        //    }
        //}

        private void SetAxisLimits(double min, double max)
        {
            AxisMin = min;
            AxisMax = max;
        }

        public double AxisMax
        {
            get { return _axisMax; }
            set
            {
                _axisMax = value;
                OnPropertyChanged();
            }
        }
        public double AxisMin
        {
            get { return _axisMin; }
            set
            {
                _axisMin = value;
                OnPropertyChanged();
            }
        }

        public ChartValues<DataPoint> LaRa { get; } = new ChartValues<DataPoint>();
        public ChartValues<DataPoint> LlRa { get; } = new ChartValues<DataPoint>();
        public ChartValues<DataPoint> VxRl { get; } = new ChartValues<DataPoint>();
        public ChartValues<DataPoint> LlLa { get; } = new ChartValues<DataPoint>();
        public ChartValues<DataPoint> Merged { get; } = new ChartValues<DataPoint>();
        public ChartValues<DataPoint> Filtered { get; } = new ChartValues<DataPoint>();
        public ChartValues<DataPoint> Detrend { get; } = new ChartValues<DataPoint>();
        public ChartValues<DataPoint> Heartrate { get; } = new ChartValues<DataPoint>();

        //public SeriesCollection Series { get; } = new SeriesCollection();

        public bool ShowLlRa
        {
            get { return m_showLlRa; }
            set
            {
                if (value == m_showLlRa)
                    return;
                m_showLlRa = value;
                OnPropertyChanged();
            }
        }

        public bool ShowLaRa
        {
            get { return m_showLaRa; }
            set 
            {
                if (value == m_showLaRa)
                    return;
                m_showLaRa = value;
                OnPropertyChanged();
            }
        }

        public bool ShowVxRl
        {
            get { return m_showVxRl; }
            set
            {
                if (value == m_showVxRl)
                    return;
                m_showVxRl = value;
                OnPropertyChanged();
            }
        }

        public bool ShowLlLa
        {
            get { return m_showLlLa; }
            set
            {
                if (value == m_showLlLa)
                    return;
                m_showLlLa = value;
                OnPropertyChanged();
            }
        }

        public bool ShowMerged
        {
            get { return m_showMerged; }
            set
            {
                if (value == m_showMerged)
                    return;
                m_showMerged = value;
                OnPropertyChanged();
            }
        }

        public bool ShowFiltered
        {
            get { return m_showFiltered; }
            set
            {
                if (value == m_showFiltered)
                    return;
                m_showFiltered = value;
                OnPropertyChanged();
            }
        }

        public bool ShowDetrend
        {
            get { return m_showDetrend; }
            set
            {
                if (value == m_showDetrend)
                    return;
                m_showDetrend = value;
                OnPropertyChanged();
            }
        }

        public bool ShowHeartrate
        {
            get { return m_showHeartrate; }
            set
            {
                if (value == m_showHeartrate)
                    return;
                m_showHeartrate = value;
                OnPropertyChanged();
            }
        }
    }
}

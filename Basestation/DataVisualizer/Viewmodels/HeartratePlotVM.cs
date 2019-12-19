using Basestation.Common.Data;
using DataVisualizer.Models;
using LiveCharts;
using LiveCharts.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataVisualizer.Viewmodels
{
    public class HeartratePlotVM : BaseVM
    {
        //Chart x axis
        private double _axisMax = 10;
        private double _axisMin = 0;

        private HeartrateSubscriber m_subscriber;

        public HeartratePlotVM(string address, string targetId)
        {
            m_subscriber = new HeartrateSubscriber(address, targetId, OnNewData);
            //m_subscriber.Buffer = Buffer;

            var mapper = Mappers.Xy<DataPoint>()
                .X(model => model.Timestamp)
                .Y(model => model.Value);

            Charting.For<DataPoint>(mapper);
        }

        public void OnNewData(HeartrateData data)
        {
            Heartrate.Add(new DataPoint { Value = data.Heartrate, Timestamp = data.Timestamp });
            while (Heartrate.Count > 1 * 8)
                Heartrate.RemoveAt(0);

            OnPropertyChanged(nameof(LastHeartrate));

            if (Heartrate.Count > 1)
                SetAxisLimits(Heartrate.First().Timestamp, Heartrate.Last().Timestamp);
        }

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

        public string LastHeartrate
        {
            get
            {
                if (Heartrate.LastOrDefault() == null)
                    return "n/a";
                return ((int)Heartrate.Last().Value).ToString();
            }
        }

        public ChartValues<DataPoint> Heartrate { get; } = new ChartValues<DataPoint>();
    }
}

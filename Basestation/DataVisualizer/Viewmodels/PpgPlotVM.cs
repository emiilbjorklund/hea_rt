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
    public class PpgPlotVM : BaseVM
    {
        //Chart x axis
        private double _axisMax = 10;
        private double _axisMin = 0;

        private PpgSubscriber m_subscriber;

        public PpgPlotVM(string address, string targetId)
        {
            m_subscriber = new PpgSubscriber(address, targetId, OnNewData);
            //m_subscriber.Buffer = Buffer;

            var mapper = Mappers.Xy<DataPoint>()
                .X(model => model.Timestamp)
                .Y(model => model.Value);

            Charting.For<DataPoint>(mapper);
        }

        public void OnNewData(PpgData data)
        {
            Ppg.Add(new DataPoint { Value = data.Ppg, Timestamp = data.Timestamp });
            while (Ppg.Count > 256 * 4)
                Ppg.RemoveAt(0);

            if (Ppg.Count > 1)
                SetAxisLimits(Ppg.First().Timestamp, Ppg.Last().Timestamp);
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

        public ChartValues<DataPoint> Ppg { get; } = new ChartValues<DataPoint>();
    }
}

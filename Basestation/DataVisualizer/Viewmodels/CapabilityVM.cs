using Basestation.Common.Abstractions;
using DataVisualizer.Views;
using System.Collections.ObjectModel;

namespace DataVisualizer.Viewmodels
{
    public class CapabilityVM : BaseVM
    {
        private Capability m_capability;
        private GrpcCallVM m_selectedCall;
        private object m_plotcontext;

        public CapabilityVM(Capability capability, ServiceVM service)
        {
            Service = service;
            m_capability = capability;

            switch (capability.Type)
            {
                case CapabilityType.DataAcquisition:
                    var da = capability as DataAcquisitionCapability;
                    foreach (var sensor in da.Sensors)
                    {
                        GrpcCallType type = GrpcCallType.EcgSubscription;
                        switch (sensor.Type)
                        {
                            case SensorType.EcgTestData:
                            case SensorType.ShimmerEcg:
                                type = GrpcCallType.EcgSubscription;
                                break;
                            case SensorType.PpgTestData:
                            case SensorType.ShimmerPpg:
                                type = GrpcCallType.PpgSubscription;
                                break;
                            default:
                                break;
                        }
                        Calls.Add(new GrpcCallVM(type, Service.Address, sensor.Id, this));
                    }
                    break;
                case CapabilityType.HeartrateEvaluation:
                    var he = capability as HeartrateEvaluationCapability;
                    Calls.Add(new GrpcCallVM(GrpcCallType.HeartrateSubscription, Service.Address, "test", this));
                    break;
                case CapabilityType.MobileAppCommunication:
                    break;
                case CapabilityType.SystemHealthMonitor:
                    break;
                case CapabilityType.WarningsAndAlerts:
                    break;
                default:
                    break;
            }
        }

        public ObservableCollection<GrpcCallVM> Calls { get; } = new ObservableCollection<GrpcCallVM>();
        public ServiceVM Service { get; }
        public GrpcCallVM SelectedCall
        {
            get => m_selectedCall;
            set
            {
                if (value == m_selectedCall)
                    return;

                m_selectedCall = value;
                OnPropertyChanged();

                if (m_selectedCall != null)
                {
                    switch (m_selectedCall.Type)
                    {
                        case GrpcCallType.EcgSubscription:
                            if (m_plotcontext == null)
                                m_plotcontext = new EcgPlotVM(m_selectedCall.Address, m_selectedCall.TargetId);
                            App.PlotContent.Content = new EcgPlot() { DataContext = m_plotcontext };
                            break;
                        case GrpcCallType.PpgSubscription:
                            if (m_plotcontext == null)
                                m_plotcontext = new PpgPlotVM(m_selectedCall.Address, m_selectedCall.TargetId);
                            App.PlotContent.Content = new PpgPlot() { DataContext = m_plotcontext };
                            break;
                        case GrpcCallType.HeartrateSubscription:
                            if (m_plotcontext == null)
                                m_plotcontext = new HeartratePlotVM(m_selectedCall.Address, m_selectedCall.TargetId);
                            App.PlotContent.Content = new HeartratePlot() { DataContext = m_plotcontext };
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public string DisplayName => m_capability.Type.ToString();
    }
}
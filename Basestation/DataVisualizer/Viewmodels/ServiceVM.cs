using Basestation.Common.Abstractions;
using System.Collections.ObjectModel;

namespace DataVisualizer.Viewmodels
{
    public class ServiceVM : BaseVM
    {
        private Service m_service;
        private CapabilityVM m_selectedCapability;

        public ServiceVM(Service service)
        {
            m_service = service;

            foreach (var cap in service.Capabilities)
                Capabilities.Add(new CapabilityVM(cap, this));
        }

        public CapabilityVM SelectedCapability
        {
            get => m_selectedCapability;
            set
            {
                if (value == m_selectedCapability)
                    return;

                if (m_selectedCapability != null)
                    m_selectedCapability.SelectedCall = null;
                m_selectedCapability = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CapabilityVM> Capabilities { get; } = new ObservableCollection<CapabilityVM>();

        public string DisplayName => $"{m_service.Hostname}:{m_service.Port}";

        public string Address => m_service.Address;
    }
}
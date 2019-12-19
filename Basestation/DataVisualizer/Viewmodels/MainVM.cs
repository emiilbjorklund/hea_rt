using Basestation.Common.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace DataVisualizer.Viewmodels
{
    public class MainVM : BaseVM
    {
        private ServiceVM m_selectedService;

        public MainVM(SystemStructure structure)
        {
            Structure = structure;

            foreach (var service in structure.Services)
                Services.Add(new ServiceVM(service));
        }

        public ObservableCollection<ServiceVM> Services { get; } = new ObservableCollection<ServiceVM>();

        public ServiceVM SelectedService
        {
            get => m_selectedService;
            set
            {
                if (value == m_selectedService)
                    return;

                if (m_selectedService != null && m_selectedService.SelectedCapability != null)
                    m_selectedService.SelectedCapability.SelectedCall = null;

                m_selectedService = value;
                OnPropertyChanged();
            }
        }

        public SystemStructure Structure { get; }

        public string Title => "Main";


        //public int MyPropertyPlusOne => myVar + 1;

        //private int myVar;

        //public int MyProperty
        //{
        //    get { return myVar; }
        //    set 
        //    {
        //        if (value == myVar)
        //            return;

        //        myVar = value;

        //        OnPropertyChanged();
        //        OnPropertyChanged(nameof(MyPropertyPlusOne));
        //    }
        //}

    }
}

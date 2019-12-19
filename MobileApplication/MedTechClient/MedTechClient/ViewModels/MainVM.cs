
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Microsoft.AspNetCore.SignalR.Client;

namespace MedTechClient
{
    class MainVM : BaseVM
    {
        public MainVM()
        {
            Client.UpdateTarget = this;
            //UpdatePulse();
           
            CheckHeartStatus();
            CheckAlarmTrigg();            
            Task.Run(DelayMessageRelay);
        }

        private async Task DelayMessageRelay()
        {
            while (true)
            {
                await Task.Delay(1000);
                if (Client.IsConnected)
                    await Client.HeartrateUpdates(OnNewHeartrate);
            }
        }

        //get heartrate into to Msg. Msg is displayed in xaml-file through binding
        private void OnNewHeartrate(double heartrate)
        {
            if (heartrate == -1)
                Msg = "n/a";
            else
                 Msg = ((int)heartrate).ToString();
        }

        public string msg ="0";
        public string Msg
        {
            get => msg;
            set
            {
                if (Msg == value)
                    return;
                msg = value;
                OnPropertyChanged();
                System.Diagnostics.Debug.WriteLine(Msg);
            }                     
        }

      
    // pulse variable for testing purposes
    //public int pulse;
    //    public int Pulse
    //    {
    //        get => pulse;
    //        set
    //        {
    //            if (pulse == value)
    //                return;
    //            pulse = value;
    //            OnPropertyChanged();
    //        }
    //    }

    //    public async void UpdatePulse()
    //    {
    //        while (true)
    //        {
    //            await Task.Delay(1000);
    //            Pulse = 85;
    //            await Task.Delay(1000);
    //            Pulse = 78;
    //        }
    //    }



       //health status enum to trigger pages for good or bad health
        private HealthStatus m_healthStatus = HealthStatus.Bad;

        public HealthStatus HealthStatus
        {
            get => m_healthStatus;
            set
            {
                if (m_healthStatus == value)
                    return;
                m_healthStatus = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HealthStatusText));
            }
        }

        public string HealthStatusText
        {
            get
            {
                switch (HealthStatus)
                {
                    case HealthStatus.Good:
                        return "Good";
                    case HealthStatus.Bad:
                        return "Bad";
                    default:
                        return "default";
                }
            }
        }

        //alarmtrigg enum to simulate lost connection and activate warning popup

        private AlarmTrigg m_alarmTrigg;
        public AlarmTrigg AlarmTrigg
        {
            get => m_alarmTrigg;
            set
            {
                if (m_alarmTrigg != value)
                    return;
                //CheckForConnection();
                //TriggerAlarm();
                m_alarmTrigg = value;
                OnPropertyChanged();
            }
        }
        public async void CheckForConnection()
        {
            if (AlarmTrigg == AlarmTrigg.ConnectionLost)
            {
                if (!PopupNavigation.PopupStack.Any())
                {
                    await PopupNavigation.Instance.PushAsync(new AlarmPopup());
                }
            }
        }

        public async void CheckAlarmTrigg()
        {
            while (true)
            {
                await Task.Delay(40 * 1000);
                AlarmTrigg = AlarmTrigg.ConnectionLost;
            }
        }

        public async void CheckHeartStatus()
        {
            // loop variable trigg alert if sertain condition
            while (true)
            {
                await Task.Delay(1000);
                if (HealthStatus == HealthStatus.Bad)
                    HealthStatus = HealthStatus.Good;
                else
                    HealthStatus = HealthStatus.Bad;
            }
        }
    }//class
}//namespace


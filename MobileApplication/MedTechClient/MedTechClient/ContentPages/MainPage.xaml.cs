
using MedTechClient.ContentPages;
using Microsoft.AspNetCore.SignalR.Client;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MedTechClient
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]

    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            Task.Run(Animate);
        }
             


        //blink heart image
        private async void Animate()
        {
            while (true)
            {
                await PulseButton.ScaleTo(0.7, 1200);
                await PulseButton.ScaleTo(1.1, 800);
                
            }
        }
      public bool blink = true;
        
        //trigged when cardiac status button clicked
        private async void Button_Pressed(object sender, EventArgs e)
        {
            blink = false;
            await PopupNavigation.Instance.PushAsync(new ActivityIndicator());

            await Task.Delay(500);
            await PopupNavigation.Instance.PopAsync();

            GoodOrBad();
            blink = true;

        }

        // Healthstatus enum, defined in "Types" folder, changed MainVM in a given time interval
        private void GoodOrBad()
        {
            var vm = BindingContext as MainVM;
            if (vm == null)
                return;

            if (vm.HealthStatus == HealthStatus.Bad)
            {
                Navigation.PushAsync(new BadHealth());
            }
            if (vm.HealthStatus == HealthStatus.Good)
            {
                Navigation.PushAsync(new GoodHealth());
            }

        }

        // open new page when button "change server Url" is pressed 
        private async void Button_Pressed_2(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ChangeServer());
        }
    }
}




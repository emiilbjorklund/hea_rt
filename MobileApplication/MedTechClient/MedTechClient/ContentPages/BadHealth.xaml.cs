using Plugin.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MedTechClient
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BadHealth : ContentPage
    {
        private object caller;

        public BadHealth()
        {
            InitializeComponent();
        }

        //open phone when button clicked
        private void Button_Clicked(object sender, EventArgs e)
        {
            var phoneDialer = CrossMessaging.Current.PhoneDialer;
            if (phoneDialer.CanMakePhoneCall)
                phoneDialer.MakePhoneCall("SOSnumber", "SOS");
        }

    }
}
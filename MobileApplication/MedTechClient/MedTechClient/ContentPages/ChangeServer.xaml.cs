using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MedTechClient.ContentPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChangeServer : ContentPage
    {
        public ChangeServer()
        {
            InitializeComponent();
        }


        //TODO: change for automaticly connect when application starts.
        private async void Button_Pressed(object sender, EventArgs e)
        {
            var url = c_URL.Text + "/heartrate";
            if (string.IsNullOrWhiteSpace(c_URL.Text))
                //url = "http://192.168.1.155:4990/heartrate";
                url = "http://192.168.43.149:4990/heartrate";

                await Client.Connect(url);

        }
    }
}
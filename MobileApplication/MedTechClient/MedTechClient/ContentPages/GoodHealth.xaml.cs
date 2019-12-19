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
    public partial class GoodHealth : ContentPage
    {
        public GoodHealth()
        {
            InitializeComponent();
            Animate();
        }

        // blink green heart image
        private async void Animate()
        {
            while (true)
            {
                await GreenHeart.ScaleTo(0.9, 700);
                await GreenHeart.ScaleTo(0.7, 1000);
            }
        }
    }
}



using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Threading;
using Rg.Plugins.Popup.Services;
using Rg.Plugins.Popup.Events;

namespace MedTechClient
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AlarmPopup : PopupPage

    {
        public AlarmPopup()
        {
            InitializeComponent();

        }
        // event callback
        public event EventHandler<object> CallbackEvent;
        protected override void OnDisappearing() => CallbackEvent?.Invoke(this, EventArgs.Empty);


        protected async private void Button_Clicked(object sender, EventArgs e)
        {
            var vm = BindingContext as MainVM;
            if (vm == null)
                return;

            else
            {
                //activity indicator
                await PopupNavigation.Instance.PushAsync(new ActivityIndicator());
                await Task.Delay(1000);
                await PopupNavigation.Instance.PopAsync();


                //change the alamtrigger to connection good
                vm.AlarmTrigg = AlarmTrigg.ConnectionFine;

                //close Error popup and open a new mainpage
                (Application.Current).MainPage = new NavigationPage(new MainPage());

                await PopupNavigation.Instance.PopAsync();



            }


        }

    }
}
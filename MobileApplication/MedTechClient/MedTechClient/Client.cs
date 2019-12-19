using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using MedTechClient.ContentPages;

//reminder for diffrent adresses//
//"http://localhost:5001/chatHub"
//emulator
//"http://10.0.2.2:5000/chathub"
//android on C2
//"http://192.168.1.35:5000/chathub"
//"http://192.168.1.155:4990/heartrate"


namespace MedTechClient 
{
    public static class Client 
    {
        private static HubConnection hub = null;       
             
        internal static MainVM UpdateTarget;

        public static bool IsConnected
        {
            get
            {
                if (hub == null)
                    return false;
                if (hub.State == HubConnectionState.Connected)
                    return true;
                return false;
            }
        }

        // create a connection to the server defined in ChangeServer.xaml.cs
        public static async Task Connect(string url)
        {
            hub = new HubConnectionBuilder()
                .WithUrl(url)
                .Build();

            await hub.StartAsync();
        }

        //update data stream
        public static async Task HeartrateUpdates(Action<double> updateAction)
        {
            if (hub.State != HubConnectionState.Connected)
                return;

            var stream = hub.StreamAsync<double>("HeartrateSubscription");

            await foreach (var d in stream)
                updateAction.Invoke(d);            
        }
    }
}



       
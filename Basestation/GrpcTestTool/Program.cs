using System;
using System.Threading.Tasks;

namespace GrpcTestTool
{
    class Program
    {
        static async Task Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            Console.WriteLine("Enter the service # to test:");
            Console.WriteLine("1. SystemHealthMonitor");
            Console.WriteLine("2. MobileAppCommunication");
            Console.WriteLine("3. DataAcquisition");
            Console.WriteLine("4. Status");
            Console.WriteLine("7. Heartrate");
            Console.WriteLine("8. Warnings And Alerts");
            var service = Console.ReadLine();

            Console.WriteLine("Specify target <ip>:<port> :");
            var target = Console.ReadLine();
            var address = $"http://{target}";

            switch (service)
            {
                case "1"://SHM
                    await ClientTests.TestShm(address);
                    break;
                case "2"://MACS
                    await ClientTests.TestMacs(address);
                    break;
                case "3"://SDAS
                    await ClientTests.TestSdas(address);
                    break;
                case "4"://Status
                    await ClientTests.TestStatus(address);
                    break;
                case "7"://Heartrate
                    await ClientTests.TestHeartrate(address);
                    break;
                case "8":
                    await ClientTests.TestWas(address);
                    break;
                default:
                    break;
            }
        }
    }
}

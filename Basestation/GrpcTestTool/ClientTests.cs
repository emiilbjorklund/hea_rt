using DataAcquisition;
using Grpc.Core;
using Grpc.Net.Client;
using LocalHealthEvaluation;
using MobileAppCommunication;
using ServiceStatus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using WarningsAndAlertsGrpc;
using static DataAcquisition.Sdas;
using static LocalHealthEvaluation.LphesHeartrate;
using static MobileAppCommunication.Macs;
using static ServiceStatus.Status;
using static SystemHealthMonitor.Shm;
using static WarningsAndAlertsGrpc.Was;

namespace GrpcTestTool
{
    public static class ClientTests
    {
        public static async Task TestShm(string address)
        {
            var channel = GrpcChannel.ForAddress(address);
            var client = new ShmClient(channel);

            Console.WriteLine("Enter the rpc #:");
            Console.WriteLine("1. StatusSubscription");
            var rpc = Console.ReadLine();

            switch (rpc)
            {
                case "1": //StatusSubscription
                    break;
                default:
                    break;
            }
        }

        public static async Task TestMacs(string address)
        {
            var channel = GrpcChannel.ForAddress(address);
            var client = new MacsClient(channel);

            Console.WriteLine("Enter the rpc #:");
            Console.WriteLine("1. Heartrate subscription");
            var rpc = Console.ReadLine();

            switch (rpc)
            {
                case "1":
                    var replies = client.HeartrateSubscription(new MobileAppCommunication.HeartrateRequest() { Id = "testing" });
                    replies.ResponseStream.MoveNext().Wait();
                    Console.WriteLine(replies.ResponseStream.Current.Heartrate.ToString());
                    //await foreach (var reply in replies.ResponseStream.ReadAllAsync())
                    //{
                    //    Console.WriteLine(reply.Heartrate.ToString());
                    //}
                    break;
                default:
                    break;
            }
        }

        internal static async Task TestWas(string address)
        {
            var channel = GrpcChannel.ForAddress(address);
            var client = new WasClient(channel);
            string testId;
            string id1 = "4AEEA45E-E554-474C-8DCD-D7AC480318F1";
            string id2 = "A74FB62A-77A3-45CC-821D-26461631D1B7";
            string id3 = "1D59E4E6-CD5C-4FD2-B435-1173B887DFD2";
            Console.WriteLine("Select sending service:");
            Console.WriteLine("1. Service 1");
            Console.WriteLine("2. Service 2");
            Console.WriteLine("3. Service 3");
            var ids = Console.ReadLine();
            switch (ids)
            {
                case "1":
                    testId = id1;
                    break;
                case "2":
                    testId = id2;
                    break;
                case "3":
                    testId = id3;
                    break;
                default:
                    testId = "id_not_defined";
                    break;
            }

            Console.WriteLine("Enter message code to send: ");
            Console.WriteLine("1. SysOk");
            Console.WriteLine("2. SysError");
            Console.WriteLine("3. ArrythmiaWarning");
            Console.WriteLine("4. SuddenWarning");
            Console.WriteLine("5. SysFailure");

            var rpc = Console.ReadLine();
            var code = Int32.Parse(rpc);
            var reply = await client.AlarmStatusAsync(new WarningRequest() { MessageCode = code, TriggeringID = testId });
            Console.WriteLine("reply code:" + reply.ResponseCode.ToString());
        }

        public static async Task TestSdas(string address)
        {
            var channel = GrpcChannel.ForAddress(address);
            var client = new SdasClient(channel);

            Console.WriteLine("Enter the rpc #:");
            Console.WriteLine("1. EcgSubscription");
            Console.WriteLine("2. PpgSubscription");
            var rpc = Console.ReadLine();

            switch (rpc)
            {
                case "1":
                    var ecgreplies = client.EcgSubscription(new EcgSubscriptionRequest() { Id = "ecgTest" });
                    await foreach (var reply in ecgreplies.ResponseStream.ReadAllAsync())
                    {
                        Console.WriteLine(reply.Timestamp.ToString());
                    }
                    break;
                case "2":
                    var ppgreplies = client.PpgSubscription(new PpgSubscriptionRequest() { Id = "ppgTest" });
                    await foreach (var reply in ppgreplies.ResponseStream.ReadAllAsync())
                    {
                        Console.WriteLine(reply.Timestamp.ToString());
                    }
                    break;
                default:
                    break;
            }
        }

        public static async Task TestHeartrate(string address)
        {
            var channel = GrpcChannel.ForAddress(address);
            var client = new LphesHeartrateClient(channel);

            Console.WriteLine("Enter the rpc #:");
            Console.WriteLine("1. Heartrate subscription");
            var rpc = Console.ReadLine();

            switch (rpc)
            {
                case "1":
                    var replies = client.HeartrateSubscription(new LocalHealthEvaluation.HeartrateRequest() { Id = "test"});
                    await foreach (var reply in replies.ResponseStream.ReadAllAsync())
                    {
                        Console.WriteLine(reply.Heartrate);
                    }
                    break;
                default:
                    break;
            }
        }

        public static async Task TestStatus(string address)
        {
            var channel = GrpcChannel.ForAddress(address);
            var client = new StatusClient(channel);

            Console.WriteLine("Enter the rpc #:");
            Console.WriteLine("1. Heartbeat");
            var rpc = Console.ReadLine();

            switch (rpc)
            {
                case "1": //Heartbeat
                    while (true)
                    {
                        var sw = new Stopwatch();
                        sw.Start();
                        var reply = await client.HeartbeatAsync(new HeartbeatRequest() { Datetime = DateTime.UtcNow.ToString("o") });
                        sw.Stop();
                        Console.WriteLine($"Responded to heartbeat within {sw.ElapsedMilliseconds}");
                        await Task.Delay(1000);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}

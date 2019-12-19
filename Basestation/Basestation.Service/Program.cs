using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Basestation.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            //  Configuration builder used to process arguments.
            //  Supported args /port=<port> , /id=<guid>, /yml=<yml file>

            //  The "port" argument is used primarily by the DevLauncher 
            //  console application to launch multiple service of the same type in a development environment.

            //  The "id" argument is used both by DevLauncher and by docker in a runtime scenario,
            //  the id is used to make the program aware of its identity as specified in the init_conf.yml,
            //  supplying information about connections etc.

            //TODO: Add yml arg description

            var config = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();

            //  Indicates service is starting
            Console.WriteLine($"Starting system service with pid {Process.GetCurrentProcess().Id}");

            //  Prints id
            var id = config.GetValue<string>("id");
            if (!string.IsNullOrWhiteSpace(id))
                Console.WriteLine($"Service id: {id}");

            //  Prints port
            var port = config.GetValue<string>("port");
            if (!string.IsNullOrWhiteSpace(port))
                Console.WriteLine($"Service port: {port}");

            //  Prints system config path
            var yml = config.GetValue<string>("yml");
            if (!string.IsNullOrWhiteSpace(yml))
                Console.WriteLine($"Using config: {Path.GetFullPath(yml)}");

            //  Start service using the config
            CreateHostBuilder(args, config).Build().Run();
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration config) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //  Startup.cs will be used to set up and map service functionality.
                    webBuilder.UseStartup<Startup>();

                    //  The config based on launch arguments will be supplied to the service to be read at a later point.
                    webBuilder.UseConfiguration(config);

                    //  If a port was specified in the launch arguments, the service will be hosted under this port on localhost.
                    //  Else the service will be hosted under the port specified in launchSettings.json under "Properties"
                    //  In a runtime scenario, the service will be hosted on port 80 in the docker container.
                    var port = config.GetValue<string>("port");
                    if (!string.IsNullOrWhiteSpace(port))
                    {
                        Console.WriteLine($"Launching on port: {port}");
                        webBuilder.UseUrls($"http://localhost:{port}");
                    }
                });
    }
}

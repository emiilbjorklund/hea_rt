using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Basestation.Common.Abstractions;
using Basestation.DataAcquisition;
using Basestation.LocalHealthEvaluation.Heartrate;
using Basestation.MobileAppCommunication.Relay;
using Basestation.Service.Services;
using Basestation.WarningsAndAlerts.Warnings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Basestation.Service
{
    public class Startup
    {
        private Guid _serviceId;
        private Capability _assignedCapability;
        private SystemStructure _structure;
        private readonly IConfiguration _config;

        //Constructor runs first
        public Startup(IConfiguration config)
        {
            //Config generated in program.cs containing launch arguments
            _config = config;

            //Using yml file specified in arguments to generate an abstraction of the system structure 
            var ymlPath = _config.GetValue<string>("yml");
            _structure = new SystemStructure(ymlPath);

            //Set id using specified argument (this may crash if id is not specified correctly)
            _serviceId = _config.GetValue<Guid>("id");

            //Get service capability using id and system structure (only one capability per service is allowed but this can be expanded with some work)
            _assignedCapability = _structure.GetCapability(_serviceId);
        }

        // ConfigureServices runs 2nd
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //Service singletons added depending on capability (only one capability per service is allowed but this can be expanded with some work)
            switch (_assignedCapability.Type)
            {
                case CapabilityType.DataAcquisition:
                    Console.WriteLine("Starting sensor manager");
                    var sm = new SensorManager(_assignedCapability as DataAcquisitionCapability);
                    services.AddSingleton<ISensorManager>(sm);
                    break;
                case CapabilityType.HeartrateEvaluation:
                    Console.WriteLine("Starting heartrate evaluator");
                    var hr = new HeartrateEvaluator(_assignedCapability as HeartrateEvaluationCapability);
                    services.AddSingleton<IHeartrateEvaluator>(hr);
                    break;
                case CapabilityType.MobileAppCommunication:
                    Console.WriteLine("Starting mobile app communication");
                    var rm = new RelayManager(_assignedCapability as MobileAppCommunicationCapability);
                    services.AddSingleton<IRelayManager>(rm);
                    services.AddControllers();
                    services.AddSignalR();
                    break;
                case CapabilityType.SystemHealthMonitor:
                    break;
                case CapabilityType.WarningsAndAlerts:
                    Console.WriteLine("Starting warnings and alerts");
                    var wa = new WarningManager(_assignedCapability as WarningsAndAlertsCapability);
                    services.AddSingleton<IWarningManager>(wa);
                    break;
                default:
                    break;
            }
            
            services.AddGrpc();
        }

        // Configure runs 3rd
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                //Depending on capability grpc services are mapped
                switch (_assignedCapability.Type)
                {
                    case CapabilityType.DataAcquisition:
                        Console.WriteLine("Mapping sdas grpc service");
                        endpoints.MapGrpcService<SdasService>();
                        break;
                    case CapabilityType.HeartrateEvaluation:
                        Console.WriteLine("Mapping heartrate grpc service");
                        endpoints.MapGrpcService<HeartrateService>();
                        break;
                    case CapabilityType.MobileAppCommunication:
                        Console.WriteLine("Mapping macs controllers and hubs");
                        endpoints.MapControllers();
                        break;
                    case CapabilityType.SystemHealthMonitor:
                        break;
                    case CapabilityType.WarningsAndAlerts:
                        endpoints.MapGrpcService<WasService>();
                        break;
                    default:
                        break;
                }

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                });
            });
        }
    }
}

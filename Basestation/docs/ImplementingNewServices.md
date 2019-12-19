# Implementing New Services

When continuing development a new service might need to be implemented, this document contains information about the process of doing this.


## System Abstractions

The system configuration file defines the complete system structure. To support a new type of service, the software that parses the configuration file need to be able to recognize it as its own seperate type. 

The system structure is parsed to a simple data structure that is avaible to software components on the platform, this data structure contains information about connection addresses, connected sensors etc. and is used when starting up the system or making new procedure calls.


### Defining new classes/types for the abstract data structure

In Basestation.Common/Abstractions/Capability.cs we need to define a new capability type, a capability defines the function of a service. This means adding a new type to the followin enum:

```C#
public enum CapabilityType
{
    DataAcquisition,
    HeartrateEvaluation,
    MobileAppCommunication,
    SystemHealthMonitor,
    WarningsAndAlerts
}
``` 

Further a new capability class will need to be defined in the 'Abstractions' folder, this class will need to be able to parse the information specified for it in the yml file. **Check the implementation of ParseYaml() in for example DataAcquisitionCapability.cs for an idea of how it is done**

In Capability.cs ParseYaml() method, implement the ability to read this new type from the yml:

```C#
public static Capability FromYaml(KeyValuePair<YamlNode, YamlNode> capabilityNode, Service servRef)
{
    var key = capabilityNode.Key;
    var val = (YamlMappingNode)capabilityNode.Value;

    if (!Enum.TryParse<CapabilityType>(key.ToString(), true, out CapabilityType type))
        throw new Exception($"Node type could not be parsed '{key.ToString()}' is not a valid node type");

    Capability cap = null;
    switch (type)
    {
        case CapabilityType.DataAcquisition:
            cap = DataAcquisitionCapability.FromYaml(val);
            break;
        case CapabilityType.HeartrateEvaluation:
            cap = HeartrateEvaluationCapability.FromYaml(val);
            break;
        case CapabilityType.MobileAppCommunication:
            cap = MobileAppCommunicationCapability.FromYaml(val);
            break;
        case CapabilityType.SystemHealthMonitor:
            break;
        case CapabilityType.WarningsAndAlerts:
            cap = WarningsAndAlertsCapability.FromYaml(val);
            break;
        //NEW TYPE HERE
        case CapabilityType.<NEW TYPE HERE>:
            cap = <NEW TYPE CLASS>.FromYaml(val)
            break;
        default:
            throw new Exception($"Node could not be parsed to a capability {key.ToString()}");
    }

    if (cap != null)
        cap.Service = servRef;
    return cap;
}
```

### Defining in yml

To be able to start a new service it needs to be defined in the system configuration yml file. The information contained in the new capability type is completely dependent on the functionality of the new service and how to parse it is also completely up to the implementation in the previous chapter.

```YML
services:
  service_name:
    id: 6F97518B-B241-4354-9DB5-0070E368A137
    active: true
    image: n/a
    hostname: localhost
    port: 5050
    capabilities:
    - <NAME OF NEW TYPE>:
        # ALL INFORMATION HERE NEEDS TO BE ABLE TO BE PARSED BY THE NEW CAPABILITY CLASS.
 ```

## Implementing Service Logic

When a new service type has been defined in the abstract system structure the service logic can be updated to support the new capability.

This is made in the following two places in startup.cs:

```C#
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
        //NEW TYPE HERE
        case CapabilityType.<NEW TYPE HERE>
            //Start service singleton here and add it as a singleton for dependency injection
            break;
        default:
            break;
    }
    
    services.AddGrpc();
}
```

```C#
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
            //NEW TYPE HERE
            case CapabilityType.<NEW TYPE HERE>
                //Map grpc services required by the system.
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
```

### Service Singleton

The actual funtionality of the capability is hosted in a singleton that can be dependency injected for the grpc services. The singleton requires an interface to be implemented for the dependency injection. 

A seperate class library should be created to seperate out the functionality of the specified capability, so the singleton and all implementation of it should be put in a project with naming aligning with the other capabilities.

### Dependency Injection

The 'services.AddSingleton' function adds a singleton for the dependency injection. This means that the grpc services that require access to the singleton can define the interface in their constructors and assuming that the singleton has been added it will be referable by the interface supplied to the services. **See service implementation Basestation.Service/Services**
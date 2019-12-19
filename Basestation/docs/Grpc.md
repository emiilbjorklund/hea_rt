# GRPC

gRPC is used to communicate between microservices in the platform. The communication interfaces are defined as .proto files located in Basestation.Common/Protos. They define rpc:s (functions) that a service shall implement, but are also used when creating a clients to define available calls to be made. 

## .NET Core implementation

In .NET Core, gRPC is supported out of the box. The proto files will be read by the framework to automatically generate code for both clients and servers. To generate these files a proto file needs to be specified in the .csproj, as either server or client like the following example in Basestation.Service.csproj:

```XML
<ItemGroup>
    <Protobuf Include="..\Basestation.Common\Protos\status.proto" GrpcServices="Server" />
    <Protobuf Include="..\Basestation.Common\Protos\sdas.proto" GrpcServices="Server" />
    <Protobuf Include="..\Basestation.Common\Protos\lphes.heartrate.proto" GrpcServices="Server" />
    <Protobuf Include="..\Basestation.Common\Protos\was.proto" GrpcServices="Server" />
  </ItemGroup>
```

This will generate code that can then be inherited from. For this solution classes that inherit from the generated service code are defined in Basestation.Service/Services. To get a better understanding of how these classes work, see dependency injection in 'ImplementingNewServices.md'

The same applies to client code, for this see instead Basestation.Common.csproj.

The code generated from this procedure is placed in the 'obj' folder in the project root folder.

## Examples

Example proto:
```proto
syntax = "proto3";

option csharp_namespace = "WarningsAndAlertsGrpc";

package WasPack;

service Was {
  rpc AlarmStatus (WarningRequest) returns (WarningResponse);
}

// integer for message code 
// OBS! shall be sorted in order of criticality, where 1 is least critcal:
// 1   - SYS OK
// 2   - SYS ERROR
// 3 - ARRHYTMIA WARNING 
// 4 - SUDDEN WARNING  
// 5 - SYS FAILURE
message WarningRequest {
  int32 messageCode = 1;
  string triggeringID = 2; // GUID of the triggering service
}

// if all ok, return the received message code
// If not:
// 999	- SYS failed to execute alert
message WarningResponse {
  int32 responseCode = 1;
}
```
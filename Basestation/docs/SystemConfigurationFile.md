# System Configuration File

## Purpose

The system configuration file defines the topology of the system,
specifies all inter-service connections, port or ip used by the service and the service capability.  

The file is parsed at program startup and used to create service objects with attributes as specified.

## Data-structure hierarchy

For correct parsing the yaml configuration file needs to be structured as the below example.
Changes to the structure imposes that the parsing functionality is altered accordingly.

```yaml
setup:
  basepath: /medtec/services 
  network:
    name: dockernet
    subnet: 172.25.0.0/16
  services:
    da1:
      id: 4CC24444-5870-4EB6-B4F2-B8120D97D785
      active: true
      image: n/a
      hostname: localhost
      port: 5000
      capabilities:
        - dataacquisition:
            sensors:
              - ecgTestData: ecgTest
              #- ShimmerEcg: COM3
    da2:
      id: D1B511AF-9B2A-4C20-BA16-B0F810B0E9BB
``` 

<!--### Hierarchy level 1
As currently implemented only *setup* exists at this level. For added functionality that is not related to setup, a different type at this level may be constructed.

### Hierarchy level 2
Contains the following -->


## Different versions
Two configuration files can be found in the Deployment directory, one for runtime execution using docker and one for local development purposes.
In development all services are located on localhost and addressed by port. On runtime using Docker, the services are instead located on the created Docker network and addressed by their specified ip-address.  

Only the development version is fully implemented and should be used as guide for finalizing the runtime version.

### Runtime specific settings
As currently implemented, for runtime using Docker.  
- Network: Subnet has to be defined, name is arbritrary.
- Hostname: Has to be defined in all services as a unique subset of the defined network subnet.
- Port: Define as 80 in all services.

### Development specific settings
As currently implemented, primarily for running locally using the DevLauncher tool.  
- Network: Will not be used.
- Hostname: Has to be defined as *localhost* in all services.
- Port: Has to be defined in all services as a unique number.

## Adding a service
Explaining the structure will be done by examplifying adding of a new service.

- Add the following indented under *services*
```yaml
services:
    newacronym:
      id: 
      active: true
      image: 
      hostname: 
      port: 
```
- Given newacronym does not impact parsing functionality. Any name will work.
- Use a GUID generator to create a new id
- Image can be parsed to set a name of the service at runtime deployment, but is not currently implemented
- Specify hostname and port according to above regarding different versions
- Add wanted capabilities as below indented under *newacronym* following port

```yaml
capabilities:
    - cabability1:
        source1:
            - GUID_OF_SOURCE_SERVICE
            - GUID_OF_OTHER_SOURCE_SERVICE
        source2:
            - GUID_OF_SOURCE_SERVICE
    - capability2:
        source1:
            - GUID_OF_SOURCE_SERVICE
```
- Copy/paste the id of the source service
- Capabilities has to be list (sequence nodes)
- New capabilities must exist as CapabilityType to be parsed
- Parsing functionality must be implemented for all new capabilities





# Runtime Deployment

As currently implemented only one image has to be built.
When launched on the runtime each container will take on the capabilities specified for it in the system configuration yaml-file.
To identify itself in the configuration file a parameter with the service ID has to be included in the launch command.
Preferably this should be retrieved by launching services through a script or dedicated program that parses the yaml-file and launches all services accordingly.

## Prerequisites

Building cross-platform images is done using Docker Buildx.
This has only been done in a Linux environment, therefore the guide that follows is based on that assumption.  

**Follow the guide for installing Docker and Buildx before proceeding with this guide.**  
  
It is important that the correct configuration file is available also in the docker container at runtime for correct setup.
Make sure Basestation.csproj includes the following with the correct filepath and that this is the same file that will be used for launching:

```<ItemGroup>
    <Content Include="..\Deployment\init_conf.yml">
      <CopyToOutputDirectory>Always</CopyToOutputDirectory>
    </Content>  
  </ItemGroup>
```

## Building Service Image

Development is done on an x86 architecture but the runtime machine is based on Arm architecture, therefore building has to be performed accordingly.<br/> 


### 1. Dockerfile 

A Dockerfile shall be present in the project root directory.  
To allow for cross-platform builds the file shall not contain any commands performed in build a build environment.  
The argument *dir* in the below example is the output directory to which the project has been published for linux-arm. 
The specified entrypoint is an example that can be used as guidance.

```
FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS runtime
ARG dir 
WORKDIR /app
COPY /output/${dir}/ .
ENTRYPOINT ["dotnet", "BuiltProjectName.dll", "--id", "IdOfService"] 
```

### 2. Publish project for arm

Publish the project for linux-arm framework and set the output directory that is used in the Dockerfile to build the image.
The following example can be used as guidance and is coherent with the Dockerfile example.
This should be executed from the root directory of the Basestation.Service project.  
```
dotnet publish -r linux-arm -c Release -o output/arm
```
*Hint: Make sure gitignore incudes the created output folder*

### 3. Build image for arm

Use Docker Buildx to build the image for arm.
The below example can be used as guidance and is coherent with previous examples. 
It is important that the image tag given as parameter -t is the same as is specified in the configuration yaml-file. 
```
sudo docker buildx build --no-cache=true --build-arg dir=arm --platform linux/arm/v7 -t <imageTagAsInYamlfile> --load .
```
*Hint: contruct a script or program that parses the configuration file and builds image accordingly*

### 4. Create image tarball

Save the image as tarball. The specified output folder should preferably be the same as where the configuration file is located.
Below example can be used as guidance. The given path shall include */wantedFileName.tar*
```
docker save -o <path for generated tar file> <imageTagAsInYamlfile>
```

### 5. Move image and peripheral files

Copy the folder containing the configuration file and image tarball to the runtime hardware (Raspberry Pi).


### 6. Load image and run as Docker containers

On the runtime hardware, the tarball has to be loaded by Docker and then launched as containers.
The following examples can be used as guidance.  
```
docker load < wantedFileName.tar
```
After loading the image will exist as the given tag from step 3 and docker run can be executed.  

Set up a network for service communication. As specified in the configuration file. 
```
docker network create -d bridge --subnet <subnetFromYaml> <networkNameFromYaml>
```
Last launch the containers.
<Id_from_yaml_parameter> is the id in the entrypoint in the dockerfile. 
Functionality for this is *not fully implemented* and has to be completed.
```
docker run --net <networkNameFromYaml> --ip <hostnameFromYaml> <Id_from_yaml_parameter> --restart always $SDAS1image
```

#### 6.1 Deployment script

The shell scripts parse_yaml.sh and serviceSetup.sh located in the Deployment folder can be used as inspiration to constructing a launch script / program.





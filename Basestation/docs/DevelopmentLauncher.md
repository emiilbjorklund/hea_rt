# Development Launcher
As the project only consists of a single ASP.NET Core service project the development launcher provides the ability to launch a full system for development purposes. This is the project called DevLauncher in the repository.

DevLauncher starts services defined in the system configuration file as seperate processes. The Visual Studio debugger can be attached to these processes if needed.

The DevLauncher console program takes the following arguments:

`-y <path>` Required, path to the yml file containing the system structure to launch (see SystemConfigurationFile.md). If the file path could not be evaluated the program will try looking for the 'Deployment' folder in the repository file structure and try to find the file specified.

`-w` If this flag is set the program will launch each service in a seperate window. Useful when launching in a desktop environment.

`-r` If this flag is set the program will try to launch the services from /bin/Release instead of /bin/Debug.

## Launching a development system
The following instructions will start the system specified in 'init_conf_dev.yml' located in the 'Deployment' folder, and each service will be hosted in a new console window.

Command line:
`dotnet run -- -y init_conf_dev.yml -w`

Visual Studio: Add to the application launch arguments, either through right-clicking the project -> Properties -> Debug, or adding to 'commandLineArgs' in launchSettings.json `-y "init_conf_dev.yml" -w`.
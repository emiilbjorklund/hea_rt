# TestTools

## GrpcTestTool
The Grpc Test Tool can be used to verify that a service is sending data and what it contains. The developed in a very manual manner in the sense that each rpc is implemented as its own method. So if a new rpc is implented some coding needs to be done in this project to support it.

The code structure is very simple, the console program reads input to decide what rpc to test and at what address it is hosted.

## Visualizer
The Visualizer is a test tool that is better suited to display sensor data as it contains plotting functionality. The Visualizer is developed on the WPF framework and as such only runs on Windows.

The code follows to an extent a MVVM pattern. This can be simplified as follows:

- Models are the instances containing data. These can be data packets or similar but also more complex objects. They contain no UI logic.

- UI Elements in the Views (.xaml) are bound to properties in their DataContext (a property available for all UI elements).

- A ViewModel (\<name>VM.cs) is assigned as the target of the DataContext property. This class contains the property the Views binds to. If the UI is to be updated when a property changes the OnPropertyChanged method must be called. The properties available in a ViewModel can be implemented to expose information available in the models but it is not a requirement.

This pattern decouples the UI from program logic, making it easy to display the same application in different manners or using different frameworks.
# I.DOT SiLA 2 C# Sample

This code is licensed under MIT-0.

## Purpose

This is a simple C# console application that implements the I.DOT SiLA 2 API. It can serve as a working example to better understand how the API can be communicated with, or as a starting point for a complete project.

## Getting started

Visual Studio 2019 or higher is recommended. The project has no additional prerequisites. Open Dx.IDot.Sila2.Samples.sln to get started.

## Client Implementation Guide

This tutorial covers the necessary steps for implementing an I.DOT SiLA 2 client in C#.

### Project Setup

Refer to `Dx.IDot.SiLA2.Samples.csproj` for target framework and required NuGet packages.

### Build gRPC glue code

1. Add the feature definition language (FDL) \*.sila.xml files from the Features folder into your project.
2. Add the required boilerplate code for transforming the FDL files to .proto files, e.g.:

```xml
  <Target Name="ProtoPreparation" BeforeTargets="PrepareForBuild">
    <Message Text="Copying Base Protos..." Importance="high"></Message>

    <Message Text="Started XmlTransformation AbortProcessService.sila.xml -> AbortProcessService.proto" Importance="high"></Message>
    <XslTransformation XslInputPath="xslt\fdl2proto.xsl" XmlInputPaths="Features\AbortProcessService.sila.xml" OutputPaths="Protos\AbortProcessService.proto" />
    <Message Text="Finished XmlTransformation AbortProcessService.sila.xml -> AbortProcessService.proto" Importance="high"></Message>

    ...
```

3. Add the same for the generation of service classes from the resulting .proto files, e.g.:

```xml
  <Target Name="ProtoGeneration" DependsOnTargets="ProtoPreparation" AfterTargets="ProtoPreparation">
    <Message Text="Compiling Protos..." Importance="high"></Message>
    <ItemGroup>
      <Protobuf Include="Protos\AbortProcessService.proto" ProtoRoot="Protos\" GrpcServices="Both" OutputDir="Services\" />
      <Protobuf Include="Protos\BarcodeReaderService.proto" ProtoRoot="Protos\" GrpcServices="Both" OutputDir="Services\" />
      <Protobuf Include="Protos\DispensingService.proto" ProtoRoot="Protos\" GrpcServices="Both" OutputDir="Services\" />

      ...
```

4. Build your client project.

Refer to `Dx.IDot.SiLA2.Samples.csproj` for an example implementation of these build steps.

### Create I.DOT Client

Refer to `ClientSample.cs` to see an example implementation of steps such as connecting to a server, Initializing a device and executing a protocol.

In this sample there are two different commands

#### Observable Command

An observable command is a command with information data that can be streamed during execution. It is translated to a server-streaming RPC. A server-streaming RPC is similar to a unary RPC, except that the server returns a stream of messages in response to a client’s request. The client completes once it receives all the server’s messages. Each command has three functions:

- SilaCommandName: The first function has the same name as the command. This command runs the main function asynchronously and returns a CommandExecutionUUID output. This output contains a GUID that can be used to query the command status and result.
- ** SilaCommandName **\_Info: Call this function to obtain the current execution status.
- ** SilaCommandName **\_Result: Call this function to fetch the function result.

Example:
 ```csharp
_dispensingServiceClient.DispenseProtocol

_dispensingServiceClient.DispenseProtocol_Info

_dispensingServiceClient.DispenseProtocol_Result
```

#### Non-Observable Command

A non-observablecommand is an RPC that will return immediately without streaming data during invocation. It has a simpler implementation on the client side.

Example:
```csharp
public virtual Get_ServerName_Responses Get_ServerName(
    Get_ServerName_Parameters request,
    Grpc.Core.Metadata headers = null,
    DateTime? deadline = null,
    CancellationToken cancellationToken = default (CancellationToken))
```

## Resources

- [Official SiLA 2 Repository](https://gitlab.com/SiLA2)
- [SiLA 2 C# Library](https://gitlab.com/SiLA2/sila_csharp)

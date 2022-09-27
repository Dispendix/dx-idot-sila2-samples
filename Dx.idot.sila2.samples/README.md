## Purpose

This is a simple C# console application for the I.DOT device to show how developers can connect to the I.DOT SiLA 2 service and call the API functionality remotely.

## Getting started

The project has no additional prerequisites. Open Dx.IDot.Sila2.Samples.sln to get started.

## New I.DOT SiLA 2 Client Development Tutorial

In this section, the process of the development of simple I.DOT console client application is described. This tutorial shows the necessary steps for implementing your own I.DOT client in C#.

## Prerequisites (Recommended)

• Visual Studio 2019 or higher

## Project Setup

Refer to `Dx.IDot.SiLA2.Samples.csproj` for target framework and required NuGet packages.

## Build gRPC glue code

1. Add the feature definition language (FDL) \*.sila.xml files from the Features folder into your project.
2. Add the required boilerplate code for transforming the FDL files to .proto files, e.g.:

```
  <Target Name="ProtoPreparation" BeforeTargets="PrepareForBuild">
    <Message Text="Copying Base Protos..." Importance="high"></Message>

    <Message Text="Started XmlTransformation AbortProcessService.sila.xml -> AbortProcessService.proto" Importance="high"></Message>
    <XslTransformation XslInputPath="xslt\fdl2proto.xsl" XmlInputPaths="Features\AbortProcessService.sila.xml" OutputPaths="Protos\AbortProcessService.proto" />
    <Message Text="Finished XmlTransformation AbortProcessService.sila.xml -> AbortProcessService.proto" Importance="high"></Message>

    ...
```

3. Add the same for the generation of service classes from the resulting .proto files, e.g.:

```
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

## Create I.DOT Client

Refer to `ClientSample.cs` to see an example implementation of steps such as connecting to a server, Initializing a device and executing a protocol.

In this sample we have two different commands

## Observable Command

An observable command is a command with information data that can be streamed during execution. It is translated to a server-streaming RPC. A server-streaming RPC is similar to a unary RPC, except that the server returns a stream of messages in response to a client’s request. The client completes once it receives all the server’s messages. Each command has three functions:

• SilaCommandName: The first function has the same name as the command. This command runs the main function asynchronously and returns a CommandExecutionUUID output. This output contains a GUID that can be used to query the command status and result.
• ** SilaCommandName **\_Info: Call this function to obtain the current execution status.
• ** SilaCommandName **\_Result: Call this function to fetch the function result.

```
TransferLiquidSiLAAsync

TransferLiquidSiLA_Info

TransferLiquidSiLA_Result
```

## Non-Observable Command

A non-observablecommand is an RPC that will return immediately without streaming data during invocation. It has a simpler implementation on the client side.

```
public virtual Get_ServerName_Responses Get_ServerName(
    Get_ServerName_Parameters request,
    Grpc.Core.Metadata headers = null,
    DateTime? deadline = null,
    CancellationToken cancellationToken = default (CancellationToken))
```

## Resources

- [Official SiLA 2 Repository](https://gitlab.com/SiLA2)
- [SiLA 2 C# Library](https://gitlab.com/SiLA2/sila_csharp)

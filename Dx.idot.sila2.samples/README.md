## Purpose
DX IDOT SiLA2 Samples is a simple C# console application sample for the I.DOT device to show how developers can connect to I.DOT SiLA 2 service and call the API functionality remotely.

## Getting started
The project has no additional prerequisites. Open Dx.IDot.Sila2.Samples.sln to get started.

## New I.DOT SiLA 2 Client Development Tutorial
In this section, the process of the development of simple I.DOT console client application is described. This tutorial shows the necessary steps for implementing your own I.DOT client in C#.

## Prerequisites
•	Visual Studio 2019

## Project Setup
1.	Create a net6.0 console or form app project.
2.	Install the SiLA2.Client, SiLA2.Core, SiLA2.Utils, Grpc.AspNetCore packages. These packages are available in the https://api.nuget.org/v3/index.json repository.


![image](https://user-images.githubusercontent.com/63784771/191930390-ea17d80c-b59b-499d-abb8-95ae00915775.png)

 
## Build gRPC glue code
1.	Add all or required part of I.DOT’s feature definition language (FDL) *.sila.xml files from the Features folder into your project.
2.	Define added FDL files as SilaFeature inside your *.csproj project file.

```
 <ItemGroup>
	<EmbeddedResource Remove="Features\**" />
  </ItemGroup>
 
  <Target Name="ProtoPreparation" BeforeTargets="PrepareForBuild">
    <Message Text="Copying Base Protos..." Importance="high"></Message>
 
    <Message Text="Started XmlTransformation AbortProcessService.sila.xml -> AbortProcessService.proto" Importance="high"></Message>
    <XslTransformation XslInputPath="xslt\fdl2proto.xsl" XmlInputPaths="Features\AbortProcessService.sila.xml" OutputPaths="Protos\AbortProcessService.proto" />
    <Message Text="Finished XmlTransformation AbortProcessService.sila.xml -> AbortProcessService.proto" Importance="high"></Message>
 
    <Message Text="Started XmlTransformation BarcodeReaderService.sila.xml -> BarcodeReaderService.proto" Importance="high"></Message>
    <XslTransformation XslInputPath="xslt\fdl2proto.xsl" XmlInputPaths="Features\BarcodeReaderService.sila.xml" OutputPaths="Protos\BarcodeReaderService.proto" />
    <Message Text="Finished XmlTransformation BarcodeReaderService.sila.xml -> BarcodeReaderService.proto" Importance="high"></Message>
 
    <Message Text="Started XmlTransformation DispensingService.sila.xml -> DispensingService.proto" Importance="high"></Message>
    <XslTransformation XslInputPath="xslt\fdl2proto.xsl" XmlInputPaths="Features\DispensingService.sila.xml" OutputPaths="Protos\DispensingService.proto" />
    <Message Text="Finished XmlTransformation DispensingService.sila.xml -> DispensingService.proto" Importance="high"></Message>
 
    <Message Text="Started XmlTransformation InitializationService.sila.xml -> InitializationService.proto" Importance="high"></Message>
    <XslTransformation XslInputPath="xslt\fdl2proto.xsl" XmlInputPaths="Features\InitializationService.sila.xml" OutputPaths="Protos\InitializationService.proto" />
    <Message Text="Finished XmlTransformation InitializationService.sila.xml -> InitializationService.proto" Importance="high"></Message>
 
    <Message Text="Started XmlTransformation InstrumentPropertiesService.sila.xml -> InstrumentPropertiesService.proto" Importance="high"></Message>
    <XslTransformation XslInputPath="xslt\fdl2proto.xsl" XmlInputPaths="Features\InstrumentPropertiesService.sila.xml" OutputPaths="Protos\InstrumentPropertiesService.proto" />
    <Message Text="Finished XmlTransformation InstrumentPropertiesService.sila.xml -> InstrumentPropertiesService.proto" Importance="high"></Message>
 
 
    <Message Text="Started XmlTransformation PlateMethodService.sila.xml -> PlateMethodService.proto" Importance="high"></Message>
    <XslTransformation XslInputPath="xslt\fdl2proto.xsl" XmlInputPaths="Features\PlateMethodService.sila.xml" OutputPaths="Protos\PlateMethodService.proto" />
    <Message Text="Finished XmlTransformation PlateMethodService.sila.xml -> PlateMethodService.proto" Importance="high"></Message>
 
    <Message Text="Started XmlTransformation ShutdownService.sila.xml -> ShutdownService.proto" Importance="high"></Message>
    <XslTransformation XslInputPath="xslt\fdl2proto.xsl" XmlInputPaths="Features\ShutdownService.sila.xml" OutputPaths="Protos\ShutdownService.proto" />
    <Message Text="Finished XmlTransformation ShutdownService.sila.xml -> ShutdownService.proto" Importance="high"></Message>
 
  </Target>
 
  <Target Name="ProtoGeneration" DependsOnTargets="ProtoPreparation" AfterTargets="ProtoPreparation">
    <Message Text="Compiling Protos..." Importance="high"></Message>
    <ItemGroup>
      <Protobuf Include="Protos\AbortProcessService.proto" ProtoRoot="Protos\" GrpcServices="Both" OutputDir="Services\" />
      <Protobuf Include="Protos\BarcodeReaderService.proto" ProtoRoot="Protos\" GrpcServices="Both" OutputDir="Services\" />
      <Protobuf Include="Protos\DispensingService.proto" ProtoRoot="Protos\" GrpcServices="Both" OutputDir="Services\" />
      <Protobuf Include="Protos\InitializationService.proto" ProtoRoot="Protos\" GrpcServices="Both" OutputDir="Services\" />
      <Protobuf Include="Protos\InstrumentPropertiesService.proto" ProtoRoot="Protos\" GrpcServices="Both" OutputDir="Services\" />
      <Protobuf Include="Protos\PlateMethodService.proto" ProtoRoot="Protos\" GrpcServices="Both" OutputDir="Services\" />
      <Protobuf Include="Protos\ShutdownService.proto" ProtoRoot="Protos\" GrpcServices="Both" OutputDir="Services\" />
    </ItemGroup>
  </Target>
```
3.	Build your client project. Sila2.Tools library to generate all required glue code for gRPC 

## Create I.DOT Client
Create a class containing all gRPC service clients from the I.DOT. Clients which are all generated with the same gRPC channel, an abstraction of long-lived connections to the remote server.  You can use the following code to create a simple sample connection. 
This sample code contains ClientSample.cs class with all functionality and steps such as connecting to a server, Initializing a device and executing a protocol. Each section has been described and commented on inside the code.


In this sample we have two different commands

## Observable Command
An observable command is a command with information data that can be streamed during execution. It is translated to a server streaming RPC. A server-streaming RPC is similar to a unary RPC, except that the server returns a stream of messages in response to a client’s request. The client completes once it receives all the server’s messages. Each command has three functions:

•	SilaCommandName: The first function has the same name as the command. This command runs the main function asynchronously and returns a CommandExecutionUUID output. This output contains a GUID that can be used to query the command status and result.
•	** SilaCommandName **_Info: Call this function to obtain the current execution status.
•	** SilaCommandName **_Result: Call this function to fetch the function result.


```
TransferLiquidSiLAAsync

TransferLiquidSiLA_Info

TransferLiquidSiLA_Result
```

## Non-Observable Command
A non-observablecommand is an RPC that will return immediately without streaming data during invocation. It  has a simpler implementation on the client side.

```
public virtual Get_ServerName_Responses Get_ServerName(
    Get_ServerName_Parameters request,
    Grpc.Core.Metadata headers = null,
    DateTime? deadline = null,
    CancellationToken cancellationToken = default (CancellationToken))
```
 

## About the SiLA 2 library
For more information about SiLA 2 library, please go to the [SiLA 2 official page](https://gitlab.com/SiLA2) and [SILA 2 Csharp Library](https://gitlab.com/SiLA2/sila_csharp).


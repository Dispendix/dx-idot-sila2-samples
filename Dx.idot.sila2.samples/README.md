## I.DOT SiLA 2 Client Development Tutorial
In this section, the process of the development of simple I.DOT console client application is described. This tutorial shows the necessary steps for implementing your own I.DOT client in C#.

## General Remarks
The connection to the I.DOT SiLA 2 server instrument should be implemented following the SiLA implementation guidelines. The I.DOT SiLA 2 server only supports client-initiated connection method. This means that the client should initiate the connection to the I.DOT SiLA 2 server. 
Any scheduler software, PMS (process management software), or LIMS (laboratory information management system) that complies with SiLA 2 standards can connect with the I.DOT SiLA 2 server. SiLA 2 runs over HTTP/2 connection. Therefore, this connection should use an internet or local DHCP connection with a router. The user has to connect  a client software that follows the SiLA 2 client implementations. As of 2021/02/17, the SiLA 2 consortium provides client implementations and SDKs in some popular programming languages such as:
•	Java
•	C#
•	Python
A SiLA 2 client can connect by entering an IP address or with SiLA server discovery. Server discovery is the easiest way for a client to access the I.DOT instrument. The server discovery feature uses multicast DNS messaging (mDNS) with the service name _sila._tcp.


## Prerequisites
•	Visual Studio 2019

## Project Setup
1.	Create a net6.0 console or form app project.
2.	Install the SiLA2.Client, SiLA2.Core, SiLA2.Utils, Grpc.AspNetCore packages. These packages are available in the https://api.nuget.org/v3/index.json repository.


![image](https://user-images.githubusercontent.com/63784771/191930390-ea17d80c-b59b-499d-abb8-95ae00915775.png)

 
## Build gRPC glue code
1.	Add all I.DOT’s feature definition language (FDL) *.sila.xml files into your project. They are:
a.	AbortProcessService.sila.xml
b.	BarcodeReaderService.sila.xml
c.	DispensingService.sila.xml
d.	InitializationService.sila.xml
e.	InstrumentPropertiesService.sila.xml
f.	PlateLoadingController.sila.xml
g.	PlateMethodService.sila.xml
h.	ShutdownService.sila.xml
i.	SiLAService.sila.xml
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
This sample code contains all functionality and steps such as connecting to a server, Initializing a device and executing a protocol. Each section has been described and commented on inside the code.

```
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Sila2.Org.Silastandard;
using SiLA2.Utils.gRPC;
using AbortController = Sila2.Dx.Idot.Sila.Feature.Abortprocessservice.V1;
using BarcodeReader = Sila2.Dx.Idot.Sila.Feature.Barcodereaderservice.V1;
using DispensingService = Sila2.Dx.Idot.Sila.Feature.Dispensingservice.V1;
using InitializationController = Sila2.Dx.Idot.Sila.Feature.Initializationservice.V1;
using InstrumentMaintenanceService = Sila2.Dx.Idot.Sila.Feature.Instrumentpropertiesservice.V1;
using PlateLoadingController = Sila2.Dx.Idot.Sila.Feature.Platemethodservice.V1;
using ShutdownController = Sila2.Dx.Idot.Sila.Feature.Shutdownservice.V1;
using SiLAService = Sila2.Org.Silastandard.Core.Silaservice.V1;

using Boolean = Sila2.Org.Silastandard.Boolean;
using Microsoft.Extensions.DependencyInjection;

public class ClientSample
{
    // Definition of all ávailable IDOT API client service
    private readonly DispensingService.DispensingService.DispensingServiceClient _dispensingServiceClient;
    private readonly InitializationController.InitializationService.InitializationServiceClient _initializationServiceClient;
    private readonly AbortController.AbortProcessService.AbortProcessServiceClient _abortProcessServiceClient;
    private readonly BarcodeReader.BarcodeReaderService.BarcodeReaderServiceClient _barcodeReaderServiceClient;
    private readonly InstrumentMaintenanceService.InstrumentPropertiesService.InstrumentPropertiesServiceClient _instrumentPropertiesServiceClient;
    private readonly PlateLoadingController.PlateMethodService.PlateMethodServiceClient _plateMethodServiceClient;
    private readonly ShutdownController.ShutdownService.ShutdownServiceClient _shutdownServiceClient;
    private readonly SiLAService.SiLAService.SiLAServiceClient _siLAServiceClient;
    private static IConfigurationRoot _configuration;

    public ClientSample()
    {
        // Sample csv protocol
        string filePath = $"{AppDomain.CurrentDomain.BaseDirectory}Resources{Path.DirectorySeparatorChar}TestSila.csv";
        GrpcChannel serverChannel = FindServerChannel().Result;

        // Initialize client services
        _initializationServiceClient = new InitializationController.InitializationService.InitializationServiceClient(serverChannel);
        _dispensingServiceClient = new DispensingService.DispensingService.DispensingServiceClient(serverChannel);
        _abortProcessServiceClient = new AbortController.AbortProcessService.AbortProcessServiceClient(serverChannel);
        _barcodeReaderServiceClient = new BarcodeReader.BarcodeReaderService.BarcodeReaderServiceClient(serverChannel);
        _instrumentPropertiesServiceClient = new InstrumentMaintenanceService.InstrumentPropertiesService.InstrumentPropertiesServiceClient(serverChannel);
        _plateMethodServiceClient = new PlateLoadingController.PlateMethodService.PlateMethodServiceClient(serverChannel);
        _shutdownServiceClient = new ShutdownController.ShutdownService.ShutdownServiceClient(serverChannel);
        _siLAServiceClient = new SiLAService.SiLAService.SiLAServiceClient(serverChannel);

        // Initialize device and execute sample protocol
        InitIDotDevice().Wait();
        DispenseProtocol(filePath);
    }

    /// <summary>
    /// This function is responsible for finding the server. By calling SearchForServers function it tries to discover the server first,
    /// and if a server doesn’t detect will try to connect to the server with the IP and port by calling GetChannel SiLA 2 function.
    /// </summary>
    /// <returns></returns>
    internal async Task<GrpcChannel> FindServerChannel()
    {
        GrpcChannel serverChannel;

        IConfigurationBuilder? configBuilder = new ConfigurationBuilder()
                                               .SetBasePath(Directory.GetCurrentDirectory())
                                               .AddJsonFile("appsettings.json", true, true);
        _configuration = configBuilder.Build();
        string? fqhn = _configuration["Connection:FQHN"];
        int port = int.Parse(_configuration["Connection:Port"]);

        var clientSetup = new SiLA2.Client.Configurator(_configuration);
        Console.WriteLine("Starting Server Discovery...");

        var serverMap = await clientSetup.SearchForServers();

        var serverType = "IDot SiLA2 Server";
        var server = serverMap.Values.FirstOrDefault(x => x.Info.Type == serverType);
        if (server != null)
        {
            Console.WriteLine($"Connecting to {server}");
            serverChannel = server.Channel;
        }
        else
        {
            Console.WriteLine($"No connection automatically discovered. Using Server-URI '{fqhn}:{port}' from appSettings.config");
            serverChannel = await clientSetup.ServiceProvider.GetService<IGrpcChannelProvider>()?.GetChannel(fqhn, port, true)!;
        }

        return serverChannel;
    }

    /// <summary>
    /// This is a helper function to query and wait for a execution command
    /// </summary>
    /// <param name="executionInfo">main executed command stream info output</param>
    /// <returns></returns>
    protected async Task WaitForExecutionCommend(AsyncServerStreamingCall<ExecutionInfo> executionInfo)
    {
        IAsyncStreamReader<ExecutionInfo>? responseStream = executionInfo.ResponseStream;
        while (await responseStream.MoveNext(new CancellationToken()))
        {
            if (responseStream.Current.CommandStatus == ExecutionInfo.Types.CommandStatus.FinishedSuccessfully ||
	                responseStream.Current.CommandStatus == ExecutionInfo.Types.CommandStatus.FinishedWithError)
	            {
	                break;
	            }
	        }
	    }
	
	}
	    
	/// <summary>
	/// The IDOT device must be restarted after connecting API to the service and initialize the IDOT device to prepare it for executing a protocol
	/// </summary>
	/// <param name="simulationMod">Switch server to the simulation mod</param>
	/// <returns></returns>
	public async Task InitIDotDevice(bool simulationMod = true)
	{
		CommandConfirmation? commandReset =
			_initializationServiceClient.Reset(new InitializationController.Reset_Parameters { SimulationMode = new Boolean { Value = simulationMod } });

		await WaitForExecutionCommend(_initializationServiceClient.Reset_Info(commandReset.CommandExecutionUUID));

		CommandConfirmation? commandInitialize = _initializationServiceClient.Initialize(new InitializationController.Initialize_Parameters());
		await WaitForExecutionCommend(_initializationServiceClient.Initialize_Info(commandInitialize.CommandExecutionUUID));
	}

	/// <summary>
	/// Dispense a CSV protcol
	/// </summary>
	/// <param name="filePath">CSV protocol file path. This file should exist on the server side</param>
	public async void DispenseProtocol(string filePath)
	{
		//This command runs asynchronously. To query the result or get the execution status you can use the return Command Execution UUID
		CommandExecutionUUID? commandID = _dispensingServiceClient
										  .DispenseProtocol(new DispensingService.DispenseProtocol_Parameters()
										  {
											  FilenamePath = new Sila2.Org.Silastandard.String() { Value = filePath }
										  })
										  .CommandExecutionUUID;

		// Wait for command execution to finish
		using (AsyncServerStreamingCall<ExecutionInfo>? call = _dispensingServiceClient.DispenseProtocol_Info(commandID))
		{
			IAsyncStreamReader<ExecutionInfo>? responseStream = call.ResponseStream;
			while (await responseStream.MoveNext())
			{
				// Query the dispense progress status and display it in the console
				ExecutionInfo? currentExecutionInfo = responseStream.Current;
				string? message =
					$"--> Command ControlTemperature    -status: {currentExecutionInfo.CommandStatus}   -remaining time: {currentExecutionInfo.EstimatedRemainingTime?.Seconds,3:###}s    -progress: {currentExecutionInfo.ProgressInfo.Value}";
				Console.ForegroundColor = ConsoleColor.DarkMagenta;
				Console.WriteLine(message);

				if (currentExecutionInfo.CommandStatus == ExecutionInfo.Types.CommandStatus.FinishedSuccessfully ||
					currentExecutionInfo.CommandStatus == ExecutionInfo.Types.CommandStatus.FinishedWithError)
				{
					break;
				}
			}

			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write("\n");
		}

	}
}

```
In this sample we have two different commands

## Observable Command
An observable command is a command with information data that can be streamed during execution. It is translated to a server streaming RPC. A server-streaming RPC is similar to a unary RPC, except that the server returns a stream of messages in response to a client’s request. The client completes once it receives all the server’s messages. Each command has three functions:

•	SilaCommandName: The first function has the same name as the command. This command runs the main function asynchronously and returns a CommandExecutionUUID output. This output contains a GUID that can be used to query the command status and result.
•	** SilaCommandName **_Info: Call this function to obtain the current execution status.
•	** SilaCommandName **_Result: Call this function to fetch the function result.


```
public virtual grpc::AsyncUnaryCall<global::Sila2.Org.Silastandard.CommandConfirmation> TransferLiquidSiLAAsync(global::Sila2.Dx.Idot.Sila.Feature.Dispensingservice.V1.TransferLiquidSiLA_Parameters request, grpc::CallOptions options)
 
public virtual grpc::AsyncServerStreamingCall<global::Sila2.Org.Silastandard.ExecutionInfo> TransferLiquidSiLA_Info(global::Sila2.Org.Silastandard.CommandExecutionUUID request, grpc::CallOptions options)
 
public virtual global::Sila2.Dx.Idot.Sila.Feature.Dispensingservice.V1.TransferLiquidSiLA_Responses TransferLiquidSiLA_Result(global::Sila2.Org.Silastandard.CommandExecutionUUID request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
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
 

## SiLA 2 Library
For more information about SiLA 2 library, please go to the [SiLA 2 official page](https://gitlab.com/SiLA2) and [SILA 2 Csharp Library](https://gitlab.com/SiLA2/sila_csharp).


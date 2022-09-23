## I.DOT SiLA 2 Client Development Tutorial 
## About this tutorial
In this document, the process of the development of simple I.DOT console client application is described. This tutorial shows the necessary steps for implementing your own I.DOT client in C#.

## General Remarks

The connection to the I.DOT SiLA 2 server instrument should be i
mplemented following the SiLA implementation guidelines. The I.DOT SiLA 2 server only supports client-initiated connection method. This means that the client should initiate the connection to the I.DOT SiLA 2 server.  
Any  scheduler software, PMS (process management software), or LIMS (laboratory information management system) that complies with SiLA 2 standards can connect with the I.DOT SiLA 2 server. SiLA 2 runs over HTTP/2 connection. Therefore, this connection should use an internet or local DHCP connection with a router. The user has to connect  a client software that follows the SiLA 2 client implementations. As of 2021/02/17, the SiLA 2 consortium provides client implementations and SDKs in some popular programming languages such as:
•	Java
•	C#
•	Python
A SiLA 2 client can connect by entering an IP aAddress or with SiLA server discovery. Server discovery is the easiest way for a client to access the I.DOT instrument. The server discovery feature uses a multicast DNS messaging feature (mDNS) with the service name _sila._tcp     .


Prerequisites
•	Visual Studio 2019
Project Setup
1.	Create a net6.0 console or form aApp project.
2.	Install the SiLA2.Client, SiLA2.Core, SiLA2.Utils, Grpc.AspNetCore packages. These packages are available in the https://api.nuget.org/v3/index.json repository.

 
Build gRPC glue codes  
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
  </Target
3.	Build your client project. Sila2.Tools library to generate all required glue code for gRPC 
Create I.DOT Client
Create a class containing all gRPC service clients from the I.DOT. Clients which are all generated with the same gRPC channel, an abstraction of long-lived connections to the remote server.  You can use the following code to create a simple sample connection. 
This sample code contains all functionality and steps such as connecting to a server, Initializing a device and executing a protocol. Each section has been described and commented on inside the code.
1.	using Grpc.Core;
2.	using Grpc.Net.Client;
3.	using Microsoft.Extensions.Configuration;
4.	using Sila2.Org.Silastandard;
5.	using SiLA2.Utils.gRPC;
6.	using AbortController = Sila2.Dx.Idot.Sila.Feature.Abortprocessservice.V1;
7.	using BarcodeReader = Sila2.Dx.Idot.Sila.Feature.Barcodereaderservice.V1;
8.	using DispensingService = Sila2.Dx.Idot.Sila.Feature.Dispensingservice.V1;
9.	using InitializationController = Sila2.Dx.Idot.Sila.Feature.Initializationservice.V1;
10.	using InstrumentMaintenanceService = Sila2.Dx.Idot.Sila.Feature.Instrumentpropertiesservice.V1;
11.	using PlateLoadingController = Sila2.Dx.Idot.Sila.Feature.Platemethodservice.V1;
12.	using ShutdownController = Sila2.Dx.Idot.Sila.Feature.Shutdownservice.V1;
13.	using SiLAService = Sila2.Org.Silastandard.Core.Silaservice.V1;
14.	 
15.	using Boolean = Sila2.Org.Silastandard.Boolean;
16.	using Microsoft.Extensions.DependencyInjection;
17.	 
18.	public class ClientSample
19.	{
20.	    // Definition of all ávailable IDOT API client service
21.	    private readonly DispensingService.DispensingService.DispensingServiceClient _dispensingServiceClient;
22.	    private readonly InitializationController.InitializationService.InitializationServiceClient _initializationServiceClient;
23.	    private readonly AbortController.AbortProcessService.AbortProcessServiceClient _abortProcessServiceClient;
24.	    private readonly BarcodeReader.BarcodeReaderService.BarcodeReaderServiceClient _barcodeReaderServiceClient;
25.	    private readonly InstrumentMaintenanceService.InstrumentPropertiesService.InstrumentPropertiesServiceClient _instrumentPropertiesServiceClient;
26.	    private readonly PlateLoadingController.PlateMethodService.PlateMethodServiceClient _plateMethodServiceClient;
27.	    private readonly ShutdownController.ShutdownService.ShutdownServiceClient _shutdownServiceClient;
28.	    private readonly SiLAService.SiLAService.SiLAServiceClient _siLAServiceClient;
29.	    private static IConfigurationRoot _configuration;
30.	 
31.	    public ClientSample()
32.	    {
33.	        // Sample csv protocol
34.	        string filePath = $"{AppDomain.CurrentDomain.BaseDirectory}Resources{Path.DirectorySeparatorChar}TestSila.csv";
35.	        GrpcChannel serverChannel = FindServerChannel().Result;
36.	 
37.	        // Initialize client services
38.	        _initializationServiceClient = new InitializationController.InitializationService.InitializationServiceClient(serverChannel);
39.	        _dispensingServiceClient = new DispensingService.DispensingService.DispensingServiceClient(serverChannel);
40.	        _abortProcessServiceClient = new AbortController.AbortProcessService.AbortProcessServiceClient(serverChannel);
41.	        _barcodeReaderServiceClient = new BarcodeReader.BarcodeReaderService.BarcodeReaderServiceClient(serverChannel);
42.	        _instrumentPropertiesServiceClient = new InstrumentMaintenanceService.InstrumentPropertiesService.InstrumentPropertiesServiceClient(serverChannel);
43.	        _plateMethodServiceClient = new PlateLoadingController.PlateMethodService.PlateMethodServiceClient(serverChannel);
44.	        _shutdownServiceClient = new ShutdownController.ShutdownService.ShutdownServiceClient(serverChannel);
45.	        _siLAServiceClient = new SiLAService.SiLAService.SiLAServiceClient(serverChannel);
46.	 
47.	        // Initialize device and execute sample protocol
48.	        InitIDotDevice().Wait();
49.	        DispenseProtocol(filePath);
50.	    }
51.	 
52.	    /// <summary>
53.	    /// This function is responsible for finding the server. By calling SearchForServers function it tries to discover the server first,
54.	    /// and if a server doesn’t detect will try to connect to the server with the IP and port by calling GetChannel SiLA 2 function.
55.	    /// </summary>
56.	    /// <returns></returns>
57.	    internal async Task<GrpcChannel> FindServerChannel()
58.	    {
59.	        GrpcChannel serverChannel;
60.	 
61.	        IConfigurationBuilder? configBuilder = new ConfigurationBuilder()
62.	                                               .SetBasePath(Directory.GetCurrentDirectory())
63.	                                               .AddJsonFile("appsettings.json", true, true);
64.	        _configuration = configBuilder.Build();
65.	        string? fqhn = _configuration["Connection:FQHN"];
66.	        int port = int.Parse(_configuration["Connection:Port"]);
67.	 
68.	        var clientSetup = new SiLA2.Client.Configurator(_configuration);
69.	        Console.WriteLine("Starting Server Discovery...");
70.	 
71.	        var serverMap = await clientSetup.SearchForServers();
72.	 
73.	        var serverType = "IDot SiLA2 Server";
74.	        var server = serverMap.Values.FirstOrDefault(x => x.Info.Type == serverType);
75.	        if (server != null)
76.	        {
77.	            Console.WriteLine($"Connecting to {server}");
78.	            serverChannel = server.Channel;
79.	        }
80.	        else
81.	        {
82.	            Console.WriteLine($"No connection automatically discovered. Using Server-URI '{fqhn}:{port}' from appSettings.config");
83.	            serverChannel = await clientSetup.ServiceProvider.GetService<IGrpcChannelProvider>()?.GetChannel(fqhn, port, true)!;
84.	        }
85.	 
86.	        return serverChannel;
87.	    }
88.	 
89.	    /// <summary>
90.	    /// This is a helper function to query and wait for a execution command
91.	    /// </summary>
92.	    /// <param name="executionInfo">main executed command stream info output</param>
93.	    /// <returns></returns>
94.	    protected async Task WaitForExecutionCommend(AsyncServerStreamingCall<ExecutionInfo> executionInfo)
95.	    {
96.	        IAsyncStreamReader<ExecutionInfo>? responseStream = executionInfo.ResponseStream;
97.	        while (await responseStream.MoveNext(new CancellationToken()))
98.	        {
99.	            if (responseStream.Current.CommandStatus == ExecutionInfo.Types.CommandStatus.FinishedSuccessfully ||
100.	                responseStream.Current.CommandStatus == ExecutionInfo.Types.CommandStatus.FinishedWithError)
101.	            {
102.	                break;
103.	            }
104.	        }
105.	    }
106.	 
107.	    /// <summary>
108.	    /// The IDOT device must be restarted after connecting API to the service and initialize the IDOT device to prepare it for executing a protocol 
109.	    /// </summary>
110.	    /// <param name="simulationMod">Switch server to the simulation mod</param>
111.	    /// <returns></returns>
112.	    public async Task InitIDotDevice(bool simulationMod = true)
113.	    {
114.	        CommandConfirmation? commandReset =
115.	            _initializationServiceClient.Reset(new InitializationController.Reset_Parameters { SimulationMode = new Boolean { Value = simulationMod } });
116.	 
117.	        await WaitForExecutionCommend(_initializationServiceClient.Reset_Info(commandReset.CommandExecutionUUID));
118.	 
119.	        CommandConfirmation? commandInitialize = _initializationServiceClient.Initialize(new InitializationController.Initialize_Parameters());
120.	        await WaitForExecutionCommend(_initializationServiceClient.Initialize_Info(commandInitialize.CommandExecutionUUID));
121.	    }
122.	 
123.	    /// <summary>
124.	    /// Dispense a CSV protcol
125.	    /// </summary>
126.	    /// <param name="filePath">CSV protocol file path. This file should exist on the server side</param>
127.	    public async void DispenseProtocol(string filePath)
128.	    {
129.	        //This command runs asynchronously. To query the result or get the execution status you can use the return Command Execution UUID
130.	        CommandExecutionUUID? commandID = _dispensingServiceClient
131.	                                          .DispenseProtocol(new DispensingService.DispenseProtocol_Parameters()
132.	                                          {
133.	                                              FilenamePath = new Sila2.Org.Silastandard.String() { Value = filePath }
134.	                                          })
135.	                                          .CommandExecutionUUID;
136.	 
137.	        // Wait for command execution to finish
138.	        using (AsyncServerStreamingCall<ExecutionInfo>? call = _dispensingServiceClient.DispenseProtocol_Info(commandID))
139.	        {
140.	            IAsyncStreamReader<ExecutionInfo>? responseStream = call.ResponseStream;
141.	            while (await responseStream.MoveNext())
142.	            {
143.	                // Query the dispense progress status and display it in the console
144.	                ExecutionInfo? currentExecutionInfo = responseStream.Current;
145.	                string? message =
146.	                    $"--> Command ControlTemperature    -status: {currentExecutionInfo.CommandStatus}   -remaining time: {currentExecutionInfo.EstimatedRemainingTime?.Seconds,3:###}s    -progress: {currentExecutionInfo.ProgressInfo.Value}";
147.	                Console.ForegroundColor = ConsoleColor.DarkMagenta;
148.	                Console.WriteLine(message);
149.	 
150.	                if (currentExecutionInfo.CommandStatus == ExecutionInfo.Types.CommandStatus.FinishedSuccessfully ||
151.	                    currentExecutionInfo.CommandStatus == ExecutionInfo.Types.CommandStatus.FinishedWithError)
152.	                {
153.	                    break;
154.	                }
155.	            }
156.	 
157.	            Console.ForegroundColor = ConsoleColor.Gray;
158.	            Console.Write("\n");
159.	        }
160.	 
161.	    }
162.	}
163.	 
164.	 
In this sample we have two different commands
Observable Command
An observable command is a command with information data that can be streamed during execution. It is translated to a server streaming RPC. A server-streaming RPC is similar to a unary RPC, except that the server returns a stream of messages in response to a client’s request. The client completes once it receives all the server’s messages. Each command has three functions:

•	SilaCommandName: The first function has the same name as theof command. This command runs the main function asyncronothly asynchronously and returns a CommandExecutionUUID output.  This output contains a GUID number that can be used to query the command status and result.
•	** SilaCommandName **_Info: By calling this function, you can haveCall this function to obtain the current execution status.
•	** SilaCommandName **_Result: The first part means the primary function name. By calling this function, you canCall this function to fetch the main function result.
public virtual grpc::AsyncUnaryCall<global::Sila2.Org.Silastandard.CommandConfirmation> TransferLiquidSiLAAsync(global::Sila2.Dx.Idot.Sila.Feature.Dispensingservice.V1.TransferLiquidSiLA_Parameters request, grpc::CallOptions options)
 
public virtual grpc::AsyncServerStreamingCall<global::Sila2.Org.Silastandard.ExecutionInfo> TransferLiquidSiLA_Info(global::Sila2.Org.Silastandard.CommandExecutionUUID request, grpc::CallOptions options)
 
public virtual global::Sila2.Dx.Idot.Sila.Feature.Dispensingservice.V1.TransferLiquidSiLA_Responses TransferLiquidSiLA_Result(global::Sila2.Org.Silastandard.CommandExecutionUUID request, grpc::Metadata headers = null, global::System.DateTime? deadline = null, global::System.Threading.CancellationToken cancellationToken = default(global::System.Threading.CancellationToken))
 

Non-Ot observable Command
A non-observablen inobservable command is an RPC that will return immediately without streaming data during invocation. Since it is a command without features like an observable command, itIt  has a simpler implementation on the client -side.
public virtual Get_ServerName_Responses Get_ServerName(
        Get_ServerName_Parameters request,
        Grpc.Core.Metadata headers = null,
        DateTime? deadline = null,
        CancellationToken cancellationToken = default (CancellationToken))
 

SiLA 2 Library
For more information about SiLA 2 library, please go to the SiLA 2 official page and SILA 2 Csharp Library.


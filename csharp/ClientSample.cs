using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Sila2.Org.Silastandard;
using SiLA2.Utils.gRPC;
using Abortprocesscontroller = Sila2.Dx.Idot.Sila.Dispensing.Abortprocesscontroller.V1;
using Barcodereaderservice = Sila2.Dx.Idot.Sila.Dispensing.Barcodereaderservice.V1;
using DispensingService = Sila2.Dx.Idot.Sila.Dispensing.Dispensingservice.V1;
using InitializationController = Sila2.Dx.Idot.Sila.Dispensing.Initializationcontroller.V1;
using Instrumentstatusprovider = Sila2.Dx.Idot.Sila.Dispensing.Instrumentstatusprovider.V1;
using PlateLoadingController = Sila2.Dx.Idot.Sila.Dispensing.Platetraycontroller.V1;
using ShutdownController = Sila2.Dx.Idot.Sila.Dispensing.Shutdowncontroller.V1;
using SiLAService = Sila2.Org.Silastandard.Core.Silaservice.V1;

using Boolean = Sila2.Org.Silastandard.Boolean;
using Microsoft.Extensions.DependencyInjection;
using Sila2.Org.Silastandard.Core.Errorrecoveryservice.V1;
using SiLA2.Server.Utils;

public class ClientSample
{
    // Definition of all ávailable IDOT API client service
    private readonly DispensingService.DispensingService.DispensingServiceClient _dispensingServiceClient;
    private readonly InitializationController.InitializationController.InitializationControllerClient _initializationControllerClient;
    private readonly Abortprocesscontroller.AbortProcessController.AbortProcessControllerClient _abortProcessControllerClient;
    private readonly Barcodereaderservice.BarcodeReaderService.BarcodeReaderServiceClient _barcodeReaderServiceClient;
    private readonly Instrumentstatusprovider.InstrumentStatusProvider.InstrumentStatusProviderClient _instrumentStatusProviderClient;
    private readonly PlateLoadingController.PlateTrayController.PlateTrayControllerClient _plateTrayControllerClient;
    private readonly ShutdownController.ShutdownController.ShutdownControllerClient _shutdownControllerClient;
    private readonly ErrorRecoveryService.ErrorRecoveryServiceClient _errorRecoveryClient;
    private readonly SiLAService.SiLAService.SiLAServiceClient _siLAServiceClient;
    private static IConfigurationRoot _configuration;

    public ClientSample()
    {
        // Sample csv protocol
        string filePath = $"{AppDomain.CurrentDomain.BaseDirectory}Resources{Path.DirectorySeparatorChar}TestSila.csv";
        // string filePath = "C:\\Users\\ValentinBusch\\I-DOT Assay Studio\\Protocols\\Test CSV Normal.csv";
        GrpcChannel serverChannel = FindServerChannel().Result;

        // Initialize client services
        _initializationControllerClient = new InitializationController.InitializationController.InitializationControllerClient(serverChannel);
        _dispensingServiceClient = new DispensingService.DispensingService.DispensingServiceClient(serverChannel);
        _abortProcessControllerClient = new Abortprocesscontroller.AbortProcessController.AbortProcessControllerClient(serverChannel);
        _barcodeReaderServiceClient = new Barcodereaderservice.BarcodeReaderService.BarcodeReaderServiceClient(serverChannel);
        _instrumentStatusProviderClient = new Instrumentstatusprovider.InstrumentStatusProvider.InstrumentStatusProviderClient(serverChannel);
        _plateTrayControllerClient = new PlateLoadingController.PlateTrayController.PlateTrayControllerClient(serverChannel);
        _shutdownControllerClient = new ShutdownController.ShutdownController.ShutdownControllerClient(serverChannel);
        _errorRecoveryClient = new ErrorRecoveryService.ErrorRecoveryServiceClient(serverChannel);

        _siLAServiceClient = new SiLAService.SiLAService.SiLAServiceClient(serverChannel);
        var serverVersion = _siLAServiceClient.Get_ServerVersion(new SiLAService.Get_ServerVersion_Parameters());
        Console.WriteLine($"Server Service Version: {serverVersion}");

        // Initialize device and execute sample protocol
        InitIDotDevice().Wait();
        DispenseProtocol(filePath).Wait();
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

        var clientSetup = new SiLA2.Client.Configurator(_configuration, new string[] {});
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

    /// <summary>
    /// The IDOT device must be restarted after connecting API to the service and initialize the IDOT device to prepare it for executing a protocol 
    /// </summary>
    /// <param name="simulationMod">Switch server to the simulation mod</param>
    /// <returns></returns>
    public async Task InitIDotDevice(bool simulationMod = true)
    {
        try
        {
            CommandConfirmation? commandReset =
                _initializationControllerClient.Reset(new InitializationController.Reset_Parameters { SimulationMode = new Boolean { Value = simulationMod } });

            using (AsyncServerStreamingCall<ExecutionInfo>? call = _initializationControllerClient.Reset_Info(commandReset.CommandExecutionUUID))
            {
                await WaitForExecutionCommend(call);
                _initializationControllerClient.Reset_Result(commandReset.CommandExecutionUUID);
            }

            CommandConfirmation? commandInitialize = _initializationControllerClient.Initialize(new InitializationController.Initialize_Parameters());
            using (AsyncServerStreamingCall<ExecutionInfo>? call = _initializationControllerClient.Initialize_Info(commandInitialize.CommandExecutionUUID))
            {
                await WaitForExecutionCommend(call);
                _initializationControllerClient.Initialize_Result(commandInitialize.CommandExecutionUUID);
            }
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            string error = ErrorHandling.HandleException(e);
            Console.WriteLine(error);
            throw;
        }
    }

    /// <summary>
    /// Dispense a CSV protcol
    /// </summary>
    /// <param name="filePath">CSV protocol file path. This file should exist on the server side</param>
    public async Task DispenseProtocol(string filePath)
    {
        Console.ForegroundColor = ConsoleColor.DarkMagenta;
        try
        {
            var instrumentStatusInitial =
                _instrumentStatusProviderClient.Get_InstrumentStatus(new Instrumentstatusprovider.Get_InstrumentStatus_Parameters());
            if (instrumentStatusInitial.InstrumentStatus.Value != "Idle")
            {
                Console.WriteLine("I.DOT to execute a protocol  should be in the Idle state.");
                return;
            }

            //This command runs asynchronously. To query the result or get the execution status you can use the return Command Execution UUID
            CommandExecutionUUID? DispenseCommandID = _dispensingServiceClient
                                              .DispenseProtocol(new DispensingService.DispenseProtocol_Parameters()
                                              {
                                                  FileNamePath = new Sila2.Org.Silastandard.String() { Value = filePath }
                                              })
                                              .CommandExecutionUUID;
            Console.WriteLine("Sent DispenseProtocol Command.");

            ////This command runs asynchronously. To query the result or get the execution status you can use the return Command Execution UUID
            //CommandExecutionUUID? commandID1 = _dispensingServiceClient
            //                                  .DispenseProtocol(new DispensingService.DispenseProtocol_Parameters()
            //                                  {
            //                                      FileNamePath = new Sila2.Org.Silastandard.String() { Value = filePath }
            //                                  })
            //                                  .CommandExecutionUUID;

            //try
            //{
            //    while (true)
            //    {
            //        var instrumentStatus1 =
            //            _instrumentStatusProviderClient.Get_InstrumentStatus(
            //                new Instrumentstatusprovider.Get_InstrumentStatus_Parameters());
            //        if (instrumentStatus1.InstrumentStatus.Value != "Idle")
            //        {
            //            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            //            Console.WriteLine("I.DOT to execute a protocol  should be in the Idle state.");
            //        }
            //        else
            //        {
            //            break;
            //        }

            //        Thread.Sleep(10);

            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.ToString());
            //}

            Thread.Sleep(1000);

            var instrumentStatusDuringDispense =
                _instrumentStatusProviderClient.Get_InstrumentStatus(
                    new Instrumentstatusprovider.Get_InstrumentStatus_Parameters());

            Console.WriteLine("The device status is " + instrumentStatusDuringDispense.ToString());

            //if (instrumentStatusDuringDispense.InstrumentStatus.Value != "Idle")
            //{
            //    Console.WriteLine("The I.DOT is still busy dispensing the protocol. Aborting dispensing...");
            //    _abortProcessControllerClient.AbortProcess(new Abortprocesscontroller.AbortProcess_Parameters());

            //    Thread.Sleep(1000);

            //    var instrumentStatusAfterAbort =
            //        _instrumentStatusProviderClient.Get_InstrumentStatus(
            //            new Instrumentstatusprovider.Get_InstrumentStatus_Parameters());
            //    Console.WriteLine("The new device status after abort is: " + instrumentStatusAfterAbort.InstrumentStatus.Value.ToString());
            //}

            
            // Wait for command execution to finish
            using (AsyncServerStreamingCall<ExecutionInfo>? call = _dispensingServiceClient.DispenseProtocol_Info(DispenseCommandID))
            {
                IAsyncStreamReader<ExecutionInfo>? responseStream = call.ResponseStream;
                var cancellationToken = new CancellationTokenSource();

                while (await responseStream.MoveNext(cancellationToken.Token))
                {
                    // Query the dispense progress status and display it in the console
                    ExecutionInfo? currentExecutionInfo = responseStream.Current;
                    string? message =
                        $"--> Command DispenseProtocol    -status: {currentExecutionInfo.CommandStatus}   -remaining time: {currentExecutionInfo.EstimatedRemainingTime?.Seconds,3:###}s    -progress: {currentExecutionInfo.ProgressInfo.Value}";
                    Console.WriteLine(message);

                    //var instrumentStatusDuringDispense2 =
                    //    _instrumentStatusProviderClient.Get_InstrumentStatus(
                    //        new Instrumentstatusprovider.Get_InstrumentStatus_Parameters());

                    //Console.WriteLine("The device status is " + instrumentStatusDuringDispense2.ToString());

                    if (currentExecutionInfo.CommandStatus == ExecutionInfo.Types.CommandStatus.FinishedSuccessfully ||
                        currentExecutionInfo.CommandStatus == ExecutionInfo.Types.CommandStatus.FinishedWithError)
                    {
                        break;
                    }

                }
            }

            var result = _dispensingServiceClient.DispenseProtocol_Result(DispenseCommandID);

            Console.WriteLine("DispenseProtocol result:" + result.ToString());
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            string error = ErrorHandling.HandleException(e);
            Console.WriteLine(error);
        }
    }

}

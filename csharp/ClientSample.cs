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
using String = Sila2.Org.Silastandard.String;
using System.ComponentModel.Design;
using IDot.SiLA2.Samples.CSharp;
using System.Diagnostics;

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
    private readonly IDOTWrapper _instrument;
    private static IConfigurationRoot _configuration;

    public ClientSample()
    {
        // Sample csv protocol
        string filePath = $"{AppDomain.CurrentDomain.BaseDirectory}Resources{Path.DirectorySeparatorChar}TestSila.csv";
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

        _instrument = new IDOTWrapper(_initializationControllerClient, _instrumentStatusProviderClient, _plateTrayControllerClient, _siLAServiceClient);

        Console.WriteLine("Initializing I.DOT: the instrument will restart, don't panic");
        _instrument.Initialize(false).Wait();
        Console.WriteLine(_instrument.GetVersion());

        while(true)
        {
            Console.WriteLine("Choose what to do (type the number and press <Enter>)");
            Console.WriteLine("1. Source Tray Stress Test");
            Console.WriteLine("2. Target Tray Stress Test");
            Console.WriteLine("3. Both Tray Stress Test");
            string? input = Console.ReadLine();
            if (input == null || input.Length ==0 ) {
                Console.WriteLine("unrecognized input");
                continue;
            }

            else {
                Console.Clear();
                Console.WriteLine("How many cycles ?");
                string? targetCycles = Console.ReadLine();
                Console.Clear();
                Int64 cycle = Int64.Parse(targetCycles);
                if (input.Trim() == "1")
                {
                    var tmr = Stopwatch.StartNew();
                    tmr.Start();
                    for (int i = 0; i < cycle; i++){
                        Console.WriteLine($"Executing Source Tray Stress Test cycle {i}/{cycle}");
                        _instrument.EjectTray(TrayType.Source).Wait();
                        _instrument.RetractTrays().Wait();
                        Console.Clear();
                    }
                    tmr.Stop();
                    Console.WriteLine($"Done in {tmr.Elapsed.TotalSeconds} second(s)");
                }
                else if (input.Trim() == "2")
                {
                    var tmr = Stopwatch.StartNew();
                    tmr.Start();
                    for (int i = 0; i < cycle; i++)
                    {
                        Console.WriteLine($"Executing Target Tray Stress Test cycle {i}/{cycle}");
                        _instrument.EjectTray(TrayType.Target).Wait();
                        _instrument.RetractTrays().Wait();
                        Console.Clear();
                    }
                    Console.WriteLine($"Done in {tmr.Elapsed.TotalSeconds} second(s)");
                }
                else if (input.Trim() == "3")
                {
                    var tmr = Stopwatch.StartNew();
                    tmr.Start();
                    for (int i = 0; i < cycle; i++)
                    {
                        Console.WriteLine($"Executing Both Tray Stress Test cycle {i}/{cycle}");
                        _instrument.EjectTray(TrayType.Target).Wait();
                        _instrument.EjectTray(TrayType.Source).Wait();
                        _instrument.RetractTrays().Wait();
                        Console.Clear();
                    }
                    Console.WriteLine($"Done in {tmr.Elapsed.TotalSeconds} second(s)");
                }

                else
                {
                    Console.WriteLine("unrecognized input");
                }
            }
        }

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
            Console.WriteLine("I'm trying");
            CommandConfirmation? commandReset =
                _initializationControllerClient.Reset(new InitializationController.Reset_Parameters { SimulationMode = new Boolean { Value = simulationMod } });
            Console.WriteLine("I'm halfway trying");
            using (AsyncServerStreamingCall<ExecutionInfo>? call = _initializationControllerClient.Reset_Info(commandReset.CommandExecutionUUID))
            {
                await WaitForExecutionCommend(call);
                _initializationControllerClient.Reset_Result(commandReset.CommandExecutionUUID);
            }
            Console.WriteLine("I'm finsihed");

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
        Console.WriteLine("finish init");
    }

    /// <summary>
    /// Dispense a CSV protcol
    /// </summary>
    /// <param name="filePath">CSV protocol file path. This file should exist on the server side</param>
    public async Task DispenseProtocol(string filePath)
    {
        try
        {
            var instrumentStatus =
                _instrumentStatusProviderClient.Get_InstrumentStatus(new Instrumentstatusprovider.Get_InstrumentStatus_Parameters());
            if (instrumentStatus.InstrumentStatus.Value != "Idle")
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("I.DOT to execute a protocol  should be in the Idle state.");
                return;
            }

            //This command runs asynchronously. To query the result or get the execution status you can use the return Command Execution UUID
            CommandExecutionUUID? commandID = _dispensingServiceClient
                                              .DispenseProtocol(new DispensingService.DispenseProtocol_Parameters()
                                              {
                                                  FileNamePath = new Sila2.Org.Silastandard.String() { Value = filePath }
                                              })
                                              .CommandExecutionUUID;

            // Wait for command execution to finish
            using (AsyncServerStreamingCall<ExecutionInfo>? call = _dispensingServiceClient.DispenseProtocol_Info(commandID))
            {
                IAsyncStreamReader<ExecutionInfo>? responseStream = call.ResponseStream;
                var cancellationToken = new CancellationTokenSource();

                while (await responseStream.MoveNext(cancellationToken.Token))
                {
                    // Query the dispense progress status and display it in the console
                    ExecutionInfo? currentExecutionInfo = responseStream.Current;
                    string? message =
                        $"--> Command DispenseProtocol    -status: {currentExecutionInfo.CommandStatus}   -remaining time: {currentExecutionInfo.EstimatedRemainingTime?.Seconds,3:###}s    -progress: {currentExecutionInfo.ProgressInfo.Value}";
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(message);

                    if (currentExecutionInfo.CommandStatus == ExecutionInfo.Types.CommandStatus.FinishedSuccessfully ||
                        currentExecutionInfo.CommandStatus == ExecutionInfo.Types.CommandStatus.FinishedWithError)
                    {
                        break;
                    }
                }
            }

            _dispensingServiceClient.DispenseProtocol_Result(commandID);
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            string error = ErrorHandling.HandleException(e);
            Console.WriteLine(error);
        }
    }
    
    public async Task SetFillVolume(string xmlSchema)
    {
        try
        {
            var instrumentStatus =
                _instrumentStatusProviderClient.Get_InstrumentStatus(new Instrumentstatusprovider.Get_InstrumentStatus_Parameters());
            if (instrumentStatus.InstrumentStatus.Value != "Idle")
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("I.DOT to execute a protocol  should be in the Idle state.");
                return;
            }
    
            //This command runs asynchronously. To query the result or get the execution status you can use the return Command Execution UUID
            CommandExecutionUUID? commandID = _dispensingServiceClient
                                              .SetFillVolume(new DispensingService.SetFillVolume_Parameters()
                                              {
                                                 FillVolumes = new Sila2.Org.Silastandard.String() { Value = xmlSchema }
                                              })
                                              .CommandExecutionUUID;
    
            // Wait for command execution to finish
            using (AsyncServerStreamingCall<ExecutionInfo>? call = _dispensingServiceClient.SetFillVolume_Info(commandID))
            {
                IAsyncStreamReader<ExecutionInfo>? responseStream = call.ResponseStream;
                var cancellationToken = new CancellationTokenSource();
    
                while (await responseStream.MoveNext(cancellationToken.Token))
                {
                    // Query the dispense progress status and display it in the console
                    ExecutionInfo? currentExecutionInfo = responseStream.Current;
                    string? message =
                        $"--> Command DispenseProtocol    -status: {currentExecutionInfo.CommandStatus}   -remaining time: {currentExecutionInfo.EstimatedRemainingTime?.Seconds,3:###}s    -progress: {currentExecutionInfo.ProgressInfo.Value}";
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(message);
    
                    if (currentExecutionInfo.CommandStatus == ExecutionInfo.Types.CommandStatus.FinishedSuccessfully ||
                        currentExecutionInfo.CommandStatus == ExecutionInfo.Types.CommandStatus.FinishedWithError)
                    {
                        break;
                    }
                }
            }
    
            _dispensingServiceClient.SetFillVolume_Result(commandID);
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            string error = ErrorHandling.HandleException(e);
            Console.WriteLine(error);
        }
    }

    public async Task SourceTrayStressTest(int repetition)
    {
        try
        {
            var instrumentStatus = _instrumentStatusProviderClient.Get_InstrumentStatus(new Instrumentstatusprovider.Get_InstrumentStatus_Parameters());
            if (instrumentStatus.InstrumentStatus.Value != "Idle")
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("Cannot Execute Source Tray Stress Test: Instrument must be Idle");
                return;
            }
            for (int i = 0; i < repetition; i++)
            {
                Console.WriteLine($"Ejecting Tray\t{i + 1}");
                var commandUUID = _plateTrayControllerClient.EjectTray(new PlateLoadingController.EjectTray_Parameters()
                {
                    PlateTray = new PlateLoadingController.DataType_TrayType()
                    {
                        TrayType = new String() { Value = "Source" }
                    }
                }).CommandExecutionUUID;
                _plateTrayControllerClient.EjectTray_Info(commandUUID);

                // Wait for command execution to finish
                using (AsyncServerStreamingCall<ExecutionInfo>? call = _plateTrayControllerClient.EjectTray_Info(commandUUID))
                {
                    IAsyncStreamReader<ExecutionInfo>? responseStream = call.ResponseStream;
                    var cancellationToken = new CancellationTokenSource();

                    while (await responseStream.MoveNext(cancellationToken.Token))
                    {
                        // Query the dispense progress status and display it in the console
                        ExecutionInfo? currentExecutionInfo = responseStream.Current;
                        string? message =
                            $"--> Command Eject Tray Soiurce   -status: {currentExecutionInfo.CommandStatus}   -remaining time: {currentExecutionInfo.EstimatedRemainingTime?.Seconds,3:###}s    -progress: {currentExecutionInfo.ProgressInfo.Value}";
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        Console.WriteLine(message);

                        if (currentExecutionInfo.CommandStatus == ExecutionInfo.Types.CommandStatus.FinishedSuccessfully ||
                            currentExecutionInfo.CommandStatus == ExecutionInfo.Types.CommandStatus.FinishedWithError)
                        {
                            break;
                        }
                    }
                }

                var  response = _plateTrayControllerClient.EjectTray_Result(commandUUID);
                Console.WriteLine(response);
                Console.WriteLine("\n");



                Console.WriteLine($"Retract Tray\t{i + 1}");
                commandUUID = _plateTrayControllerClient.RetractTrays(new PlateLoadingController.RetractTrays_Parameters()
                {
                    
                }).CommandExecutionUUID;
                _plateTrayControllerClient.RetractTrays_Info(commandUUID);

                // Wait for command execution to finish
                using (AsyncServerStreamingCall<ExecutionInfo>? call = _plateTrayControllerClient.RetractTrays_Info(commandUUID))
                {
                    IAsyncStreamReader<ExecutionInfo>? responseStream = call.ResponseStream;
                    var cancellationToken = new CancellationTokenSource();

                    while (await responseStream.MoveNext(cancellationToken.Token))
                    {
                        // Query the dispense progress status and display it in the console
                        ExecutionInfo? currentExecutionInfo = responseStream.Current;
                        string? message =
                            $"--> Command Retract Trays   -status: {currentExecutionInfo.CommandStatus}   -remaining time: {currentExecutionInfo.EstimatedRemainingTime?.Seconds,3:###}s    -progress: {currentExecutionInfo.ProgressInfo.Value}";
                        Console.ForegroundColor = ConsoleColor.DarkMagenta;
                        Console.WriteLine(message);

                        if (currentExecutionInfo.CommandStatus == ExecutionInfo.Types.CommandStatus.FinishedSuccessfully ||
                            currentExecutionInfo.CommandStatus == ExecutionInfo.Types.CommandStatus.FinishedWithError)
                        {
                            break;
                        }
                    }
                }

                var responseRet = _plateTrayControllerClient.RetractTrays_Result(commandUUID);
                Console.WriteLine(response);
                Console.WriteLine("\n");
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

    public async Task TransferLiquid(string dispenseXmlSchema, bool optimizeDispenseStepOrder)
    {
         try
        {
            var instrumentStatus =
                _instrumentStatusProviderClient.Get_InstrumentStatus(new Instrumentstatusprovider.Get_InstrumentStatus_Parameters());
            if (instrumentStatus.InstrumentStatus.Value != "Idle")
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("I.DOT to execute a protocol  should be in the Idle state.");
                return;
            }

            //This command runs asynchronously. To query the result or get the execution status you can use the return Command Execution UUID
            CommandExecutionUUID? commandID = _dispensingServiceClient
                                              .TransferLiquid(new DispensingService.TransferLiquid_Parameters()
                                              {
                                                  //FileNamePath = new Sila2.Org.Silastandard.String() { Value = filePath }
                                                  DispenseStepXmlSchema = new String(){Value = dispenseXmlSchema},
                                                  OptimizeDispenseStepOrder = new Boolean() { Value = optimizeDispenseStepOrder }
                                              })
                                              .CommandExecutionUUID;

            // Wait for command execution to finish
            using (AsyncServerStreamingCall<ExecutionInfo>? call = _dispensingServiceClient.TransferLiquid_Info(commandID))
            {
                IAsyncStreamReader<ExecutionInfo>? responseStream = call.ResponseStream;
                var cancellationToken = new CancellationTokenSource();

                while (await responseStream.MoveNext(cancellationToken.Token))
                {
                    // Query the dispense progress status and display it in the console
                    ExecutionInfo? currentExecutionInfo = responseStream.Current;
                    string? message =
                        $"--> Command TransferLiquid   -status: {currentExecutionInfo.CommandStatus}   -remaining time: {currentExecutionInfo.EstimatedRemainingTime?.Seconds,3:###}s    -progress: {currentExecutionInfo.ProgressInfo.Value}";
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(message);

                    if (currentExecutionInfo.CommandStatus == ExecutionInfo.Types.CommandStatus.FinishedSuccessfully ||
                        currentExecutionInfo.CommandStatus == ExecutionInfo.Types.CommandStatus.FinishedWithError)
                    {
                        break;
                    }
                }
            }

            DispensingService.TransferLiquid_Responses? response = _dispensingServiceClient.TransferLiquid_Result(commandID);
            Console.WriteLine(response.TransferLiquidResult.Value);
            Console.WriteLine("\n");
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            string error = ErrorHandling.HandleException(e);
            Console.WriteLine(error);
            Console.WriteLine("\n");
        }
    }

}

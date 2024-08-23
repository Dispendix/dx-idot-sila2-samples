using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Sila2.Org.Silastandard;
using SiLA2.Utils.gRPC;
using Abortprocesscontroller = Sila2.Dx.Idot.Sila.Dispensing.Abortprocesscontroller.V1;
using Barcodereaderservice = Sila2.Dx.Idot.Sila.Dispensing.Barcodereaderservice.V1;
using DispensingService = Sila2.Dx.Idot.Sila.Dispensing.Dispensingservice.V1;
using InitializationController = Sila2.Dx.Idot.Sila.Dispensing.Initializationcontroller.V1;
using InstrumentStatusProvider = Sila2.Dx.Idot.Sila.Dispensing.Instrumentstatusprovider.V1;
using PlateLoadingController = Sila2.Dx.Idot.Sila.Dispensing.Platetraycontroller.V1;
using ShutdownController = Sila2.Dx.Idot.Sila.Dispensing.Shutdowncontroller.V1;
using SiLAService = Sila2.Org.Silastandard.Core.Silaservice.V1;

using Boolean = Sila2.Org.Silastandard.Boolean;
using Microsoft.Extensions.DependencyInjection;
using Sila2.Org.Silastandard.Core.Errorrecoveryservice.V1;
using SiLA2.Server.Utils;
using String = Sila2.Org.Silastandard.String;
using System.ComponentModel.Design;
using Sila2.Dx.Idot.Sila.Dispensing.Instrumentstatusprovider.V1;
using static Sila2.Dx.Idot.Sila.Dispensing.Instrumentstatusprovider.V1.InstrumentStatusProvider;
using static Sila2.Dx.Idot.Sila.Dispensing.Platetraycontroller.V1.PlateTrayController;
using Sila2.Dx.Idot.Sila.Dispensing.Platetraycontroller.V1;

namespace IDot.SiLA2.Samples.CSharp
{
    enum IDOTStatus
    { 
        NotInitialized,
        Standby, 
        InError, 
        Idle,
        Dispensing,
        DispensingPauseRequested
    }
    enum TrayType { 
    Source,
    Target
    }
    internal class IDOTWrapper
    {   
        private InitializationController.InitializationController.InitializationControllerClient _initializationControllerClient;
        private InstrumentStatusProvider.InstrumentStatusProvider.InstrumentStatusProviderClient _instrumentStatusProvider;
        private PlateLoadingController.PlateTrayController.PlateTrayControllerClient _plateController;
        private SiLAService.SiLAService.SiLAServiceClient _silaServiceClient;

        public IDOTWrapper(
             InitializationController.InitializationController.InitializationControllerClient initController,
             InstrumentStatusProvider.InstrumentStatusProvider.InstrumentStatusProviderClient statusProvider,
             PlateLoadingController.PlateTrayController.PlateTrayControllerClient plateController,
             SiLAService.SiLAService.SiLAServiceClient silaClient
             ) {
            _plateController = plateController;
            _silaServiceClient = silaClient;
            _initializationControllerClient = initController;
            _instrumentStatusProvider = statusProvider;
        }

        public async Task Initialize(bool simulationMode = true) {
            try
            {
                var commandUUID = _initializationControllerClient.Reset(new InitializationController.Reset_Parameters { 
                    SimulationMode = new Boolean { 
                        Value = simulationMode 
                    }
                }).CommandExecutionUUID;
                await WaitForExecutionCommend(_initializationControllerClient.Reset_Info(commandUUID));

                commandUUID = _initializationControllerClient.Initialize(new InitializationController.Initialize_Parameters()).CommandExecutionUUID;
                await WaitForExecutionCommend(_initializationControllerClient.Initialize_Info(commandUUID));
                
            }
            catch (Exception e)
            {
                Console.WriteLine($"I.DOT SiLA2 client sample can not connect to the I.DOT SiLA2 server: {e.ToString}");
                return;
            }
        }

        public string GetVersion() {
            var serverVersion = _silaServiceClient.Get_ServerVersion(new SiLAService.Get_ServerVersion_Parameters());
            return serverVersion.ToString();
        }

        public IDOTStatus GetStatus() {
            var instrumentStatus = _instrumentStatusProvider.Get_InstrumentStatus(new InstrumentStatusProvider.Get_InstrumentStatus_Parameters());
            return Enum.Parse<IDOTStatus>(instrumentStatus.InstrumentStatus.Value, true);
        }
        
        public async Task EjectTray(TrayType tray) {
            Console.WriteLine($"Start Ejecting {tray.ToString()} Trays");
            var commandUUID = _plateController.EjectTray(new PlateLoadingController.EjectTray_Parameters()
            {
                PlateTray = new PlateLoadingController.DataType_TrayType()
                {
                    TrayType = new String() { Value = tray.ToString() }
                }
            }).CommandExecutionUUID;

           await WaitForExecutionCommend(_plateController.EjectTray_Info(commandUUID));
            Console.WriteLine($"Done Ejecting {tray.ToString()} Trays");

        }

        public async Task RetractTrays() {
            Console.WriteLine("Start Retracting Trays");
            var commandUUID = _plateController.RetractTrays(new PlateLoadingController.RetractTrays_Parameters(){}).CommandExecutionUUID;
            await WaitForExecutionCommend(_plateController.RetractTrays_Info(commandUUID));
            Console.WriteLine("Done Retracting Trays");

        }

        protected async Task<bool> WaitForExecutionCommend(AsyncServerStreamingCall<ExecutionInfo> executionInfo)
        {
            IAsyncStreamReader<ExecutionInfo>? responseStream = executionInfo.ResponseStream;
            while (await responseStream.MoveNext(new CancellationToken()))
            {
                switch (responseStream.Current.CommandStatus)
                {
                    case ExecutionInfo.Types.CommandStatus.Waiting:
                        continue;
                    case ExecutionInfo.Types.CommandStatus.Running:
                        continue;
                    case ExecutionInfo.Types.CommandStatus.FinishedSuccessfully:
                        return true;
                    case ExecutionInfo.Types.CommandStatus.FinishedWithError:
                        return false;
                    default:
                        break;
                }
            }
            //undetermined state
            throw new Exception("inconclusive execution info");
        }
    }
}

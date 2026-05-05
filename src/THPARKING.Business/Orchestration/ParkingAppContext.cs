using THPARKING.Business.InOut;
using THPARKING.Business.Lanes;
using THPARKING.Camera.Providers;
using THPARKING.Device.CardReaders;

namespace THPARKING.Business.Orchestration
{
    public class ParkingAppContext
    {
        public string OperatorCode { get; set; }

        public string MachineName { get; set; }

        public LaneRouterService LaneRouterService { get; set; }

        public CardCodeService CardCodeService { get; set; }

        public IParkingCameraService PlateCameraService { get; set; }

        public IParkingCameraService FaceCameraService { get; set; }

        public InOutBusinessService InOutBusinessService { get; set; }
    }
}
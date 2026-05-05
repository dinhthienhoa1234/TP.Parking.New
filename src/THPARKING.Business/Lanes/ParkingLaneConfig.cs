using THPARKING.Core.Enums;

namespace THPARKING.Business.Lanes
{
    public class ParkingLaneConfig
    {
        public string LaneCode { get; set; }

        public LaneSide Side { get; set; }

        public LaneDirection Direction { get; set; }

        public string ReaderPortName { get; set; }

        public string CameraDeviceCode { get; set; }

        public int FaceCameraChannel { get; set; }

        public int PlateCameraChannel { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
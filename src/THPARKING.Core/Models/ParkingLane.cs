using System;
using THPARKING.Core.Enums;

namespace THPARKING.Core.Models
{
    public class ParkingLane
    {
        public Guid ParkingLaneId { get; set; }

        public string LaneCode { get; set; }

        public LaneSide Side { get; set; }

        public LaneDirection Direction { get; set; }

        public string CardReaderCode { get; set; }

        public string CameraDeviceCode { get; set; }

        public int FaceCameraChannel { get; set; }

        public int PlateCameraChannel { get; set; }

        public bool IsActive { get; set; }
    }
}
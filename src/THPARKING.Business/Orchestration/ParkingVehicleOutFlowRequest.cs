using System;

namespace THPARKING.Business.Orchestration
{
    public class ParkingVehicleOutFlowRequest
    {
        public string RawCardCode { get; set; }

        public string PlateNumber { get; set; }

        public string LaneCode { get; set; }

        public string ReaderCode { get; set; }

        public string ReaderPortName { get; set; }

        public string CameraDeviceCode { get; set; }

        public int PlateCameraChannel { get; set; }

        public int FaceCameraChannel { get; set; }

        public DateTime? OriginalTimeIn { get; set; }

        public string OriginalPlateNumberNormalized { get; set; }

        public bool IsEmergencyExit { get; set; }

        public string EmergencyReason { get; set; }

        public string ImageOutPath { get; set; }

        public string FaceImageOutPath { get; set; }

        public DateTime RequestedAt { get; set; }
    }
}
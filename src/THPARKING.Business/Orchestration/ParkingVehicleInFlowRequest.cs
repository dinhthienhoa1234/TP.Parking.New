using System;

namespace THPARKING.Business.Orchestration
{
    public class ParkingVehicleInFlowRequest
    {
        public string RawCardCode { get; set; }

        public string PlateNumber { get; set; }

        public string LaneCode { get; set; }

        public string ReaderCode { get; set; }

        public string ReaderPortName { get; set; }

        public string CameraDeviceCode { get; set; }

        public int PlateCameraChannel { get; set; }

        public int FaceCameraChannel { get; set; }

        public string ImageInPath { get; set; }

        public string FaceImageInPath { get; set; }

        public DateTime RequestedAt { get; set; }
    }
}
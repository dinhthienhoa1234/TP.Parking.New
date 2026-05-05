using System;

namespace THPARKING.Business.InOut
{
    public class VehicleInRequest
    {
        public string CardCode { get; set; }

        public string CardCodeNormalized { get; set; }

        public string PlateNumber { get; set; }

        public string PlateNumberNormalized { get; set; }

        public string LaneCode { get; set; }

        public string ReaderCode { get; set; }

        public string CameraDeviceCode { get; set; }

        public string OperatorCode { get; set; }

        public string MachineName { get; set; }

        public DateTime TimeIn { get; set; }

        public string ImageInPath { get; set; }

        public string FaceImageInPath { get; set; }
    }
}
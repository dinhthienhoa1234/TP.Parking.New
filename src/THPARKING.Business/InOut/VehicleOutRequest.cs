using System;

namespace THPARKING.Business.InOut
{
    public class VehicleOutRequest
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

        public DateTime TimeOut { get; set; }

        public DateTime? OriginalTimeIn { get; set; }

        public string OriginalPlateNumberNormalized { get; set; }

        public bool IsEmergencyExit { get; set; }

        public string EmergencyReason { get; set; }

        public string ImageOutPath { get; set; }

        public string FaceImageOutPath { get; set; }
    }
}
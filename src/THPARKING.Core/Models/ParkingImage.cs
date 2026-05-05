using System;

namespace THPARKING.Core.Models
{
    public class ParkingImage
    {
        public Guid ParkingImageId { get; set; }

        public Guid ParkingSessionId { get; set; }

        public string ImageType { get; set; }

        public string LocalFilePath { get; set; }

        public string RemoteFileUrl { get; set; }

        public string ProviderName { get; set; }

        public int CameraChannel { get; set; }

        public DateTime CapturedAt { get; set; }

        public bool IsSynced { get; set; }
    }
}
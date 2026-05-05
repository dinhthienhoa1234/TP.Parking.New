using THPARKING.Core.Enums;

namespace THPARKING.Camera.Providers
{
    public class CameraConfig
    {
        public string CameraCode { get; set; }

        public string CameraName { get; set; }

        public CameraProviderType ProviderType { get; set; }

        public string IpAddress { get; set; }

        public int Port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public int Channel { get; set; }

        public string StreamUrl { get; set; }

        public int SnapshotTimeoutMilliseconds { get; set; } = 3000;

        public bool IsActive { get; set; } = true;
    }
}
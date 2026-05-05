using System;
using System.Drawing;

namespace THPARKING.Camera.Providers
{
    public class CameraSnapshotResult
    {
        public bool Success { get; private set; }

        public string CameraCode { get; private set; }

        public int Channel { get; private set; }

        public byte[] JpegBytes { get; private set; }

        public Bitmap Bitmap { get; private set; }

        public DateTime CapturedAt { get; private set; }

        public string ErrorMessage { get; private set; }

        public static CameraSnapshotResult Ok(
            string cameraCode,
            int channel,
            byte[] jpegBytes,
            Bitmap bitmap)
        {
            return new CameraSnapshotResult
            {
                Success = true,
                CameraCode = cameraCode,
                Channel = channel,
                JpegBytes = jpegBytes,
                Bitmap = bitmap,
                CapturedAt = DateTime.Now
            };
        }

        public static CameraSnapshotResult Fail(
            string cameraCode,
            int channel,
            string errorMessage)
        {
            return new CameraSnapshotResult
            {
                Success = false,
                CameraCode = cameraCode,
                Channel = channel,
                CapturedAt = DateTime.Now,
                ErrorMessage = errorMessage
            };
        }
    }
}
using System;
using System.Drawing;

namespace THPARKING.Camera.Providers
{
    public class CameraFrameResult
    {
        public bool Success { get; private set; }

        public string CameraCode { get; private set; }

        public int Channel { get; private set; }

        public Bitmap Frame { get; private set; }

        public DateTime ReceivedAt { get; private set; }

        public string ErrorMessage { get; private set; }

        public static CameraFrameResult Ok(
            string cameraCode,
            int channel,
            Bitmap frame)
        {
            return new CameraFrameResult
            {
                Success = true,
                CameraCode = cameraCode,
                Channel = channel,
                Frame = frame,
                ReceivedAt = DateTime.Now
            };
        }

        public static CameraFrameResult Fail(
            string cameraCode,
            int channel,
            string errorMessage)
        {
            return new CameraFrameResult
            {
                Success = false,
                CameraCode = cameraCode,
                Channel = channel,
                ReceivedAt = DateTime.Now,
                ErrorMessage = errorMessage
            };
        }
    }
}
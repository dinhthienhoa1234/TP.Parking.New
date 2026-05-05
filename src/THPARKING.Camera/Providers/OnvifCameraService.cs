using System;

namespace THPARKING.Camera.Providers
{
    public class OnvifCameraService : IParkingCameraService
    {
        public string CameraCode
        {
            get { return Config == null ? null : Config.CameraCode; }
        }

        public bool IsConnected { get; private set; }

        public CameraConfig Config { get; private set; }

        public void Connect(CameraConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            Config = config;

            // TODO: Integrate ONVIF client here.
            // Expected later:
            // - Get device capabilities
            // - Get media profile
            // - Get snapshot URI
            IsConnected = true;
        }

        public void Disconnect()
        {
            IsConnected = false;
        }

        public CameraSnapshotResult CaptureSnapshot(int channel)
        {
            if (!IsConnected)
            {
                return CameraSnapshotResult.Fail(
                    CameraCode,
                    channel,
                    "ONVIF camera is not connected.");
            }

            // TODO: Use ONVIF snapshot URI then download JPEG bytes.
            return CameraSnapshotResult.Fail(
                CameraCode,
                channel,
                "ONVIF snapshot is not implemented yet.");
        }

        public CameraFrameResult GetCurrentFrame(int channel)
        {
            if (!IsConnected)
            {
                return CameraFrameResult.Fail(
                    CameraCode,
                    channel,
                    "ONVIF camera is not connected.");
            }

            // TODO: Use RTSP stream reader later if needed.
            return CameraFrameResult.Fail(
                CameraCode,
                channel,
                "ONVIF live frame is not implemented yet.");
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
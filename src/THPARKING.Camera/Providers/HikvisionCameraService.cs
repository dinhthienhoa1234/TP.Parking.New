using System;

namespace THPARKING.Camera.Providers
{
    public class HikvisionCameraService : IParkingCameraService
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

            // TODO: Integrate Hikvision SDK login here.
            // Expected later:
            // - NET_DVR_Init
            // - NET_DVR_Login_V40
            // - Store user id / login handle
            IsConnected = true;
        }

        public void Disconnect()
        {
            // TODO: Logout Hikvision SDK handle here.
            IsConnected = false;
        }

        public CameraSnapshotResult CaptureSnapshot(int channel)
        {
            if (!IsConnected)
            {
                return CameraSnapshotResult.Fail(
                    CameraCode,
                    channel,
                    "Hikvision camera is not connected.");
            }

            // TODO: Replace with Hikvision JPEG capture SDK.
            return CameraSnapshotResult.Fail(
                CameraCode,
                channel,
                "Hikvision snapshot SDK is not implemented yet.");
        }

        public CameraFrameResult GetCurrentFrame(int channel)
        {
            if (!IsConnected)
            {
                return CameraFrameResult.Fail(
                    CameraCode,
                    channel,
                    "Hikvision camera is not connected.");
            }

            // TODO: Replace with Hikvision live view callback.
            return CameraFrameResult.Fail(
                CameraCode,
                channel,
                "Hikvision live frame SDK is not implemented yet.");
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
using System;

namespace THPARKING.Camera.Providers
{
    public class DahuaCameraService : IParkingCameraService
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

            // TODO: Integrate Dahua SDK login here.
            // Expected later:
            // - CLIENT_Init
            // - CLIENT_LoginEx2 / CLIENT_LoginWithHighLevelSecurity
            // - Store login handle
            IsConnected = true;
        }

        public void Disconnect()
        {
            // TODO: Logout Dahua SDK handle here.
            IsConnected = false;
        }

        public CameraSnapshotResult CaptureSnapshot(int channel)
        {
            if (!IsConnected)
            {
                return CameraSnapshotResult.Fail(
                    CameraCode,
                    channel,
                    "Dahua camera is not connected.");
            }

            // TODO: Replace with Dahua CLIENT_SnapPictureEx + callback.
            return CameraSnapshotResult.Fail(
                CameraCode,
                channel,
                "Dahua snapshot SDK is not implemented yet.");
        }

        public CameraFrameResult GetCurrentFrame(int channel)
        {
            if (!IsConnected)
            {
                return CameraFrameResult.Fail(
                    CameraCode,
                    channel,
                    "Dahua camera is not connected.");
            }

            // TODO: Replace with Dahua realplay frame callback.
            return CameraFrameResult.Fail(
                CameraCode,
                channel,
                "Dahua live frame SDK is not implemented yet.");
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
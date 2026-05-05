using System;

namespace THPARKING.Camera.Providers
{
    public interface IParkingCameraService : IDisposable
    {
        string CameraCode { get; }

        bool IsConnected { get; }

        CameraConfig Config { get; }

        void Connect(CameraConfig config);

        void Disconnect();

        CameraSnapshotResult CaptureSnapshot(int channel);

        CameraFrameResult GetCurrentFrame(int channel);
    }
}
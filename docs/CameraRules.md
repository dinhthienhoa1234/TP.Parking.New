# Camera Rules

## Supported Providers

The parking application must support these camera providers:

- Dahua
- Hikvision
- ONVIF

## Common Interface

All camera providers must implement:

- IParkingCameraService

The UI must never call Dahua, Hikvision, or ONVIF SDK directly.

## Snapshot Policy

Each snapshot result must provide:

- FullImage
- CenterCrop320x480
- UiImage
- AnprImage
- JpegBytes if available
- ProviderName
- Channel
- CaptureTime
- ErrorMessage if failed

## Image Usage

- FullImage is used for long-term storage.
- UiImage uses CenterCrop320x480.
- AnprImage uses CenterCrop320x480.
- FullImage must not be resized before storage.
- ANPR image must not be stretched or distorted.

## Crop Modes

Supported crop modes:

- Center
- BottomCenter
- ConfigRegion

Default crop mode:

- Center 320x480

If plate is not centered, use:

- BottomCenter
- ConfigRegion

## Dahua

Dahua provider rules:

- Use Dahua SDK for live view.
- Prefer JPEG callback snapshot if available.
- Fallback to file-based DHCapturePicture if callback fails.
- Do not call Dahua SDK from UI.
- Dispose native handles safely.
- Reconnect when camera is disconnected.

## Hikvision

Hikvision provider rules:

- Use HCNetSDK for live view.
- Use NET_DVR_Init before login.
- Use NET_DVR_Login_V30 for login.
- Use NET_DVR_RealPlay_V30 for live view.
- Use NET_DVR_StopRealPlay to stop live view.
- Use NET_DVR_Logout when disconnecting.
- Prefer NET_DVR_CaptureJPEGPicture for snapshot.
- Use device start channel when calculating SDK channel.
- Do not hard-code channel 0 for all devices.
- Do not call Hikvision SDK from UI.

## ONVIF

ONVIF provider rules:

- Use ONVIF to get device profiles.
- Use ONVIF GetStreamUri for RTSP live view.
- Use ONVIF GetSnapshotUri for JPEG snapshot.
- Support manual RTSP URL fallback.
- Support manual Snapshot URL fallback.
- ONVIF live view should use RTSP player component.
- Do not implement ONVIF logic inside UI.

## Live View

- Live view should use sub stream when possible.
- Snapshot should use main stream or high quality snapshot.
- Live view failure must not crash the application.
- Snapshot failure must return CameraSnapshotResult with Success = false.

## Camera Configuration

Camera configuration must be stored in database:

- CameraDeviceConfig
- LaneCameraMap

Each lane must define:

- CameraDeviceCode
- FaceCameraChannel
- PlateCameraChannel
- CropMode
- Crop region if needed
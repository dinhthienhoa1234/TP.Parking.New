# TP.Parking.New - Codex Instructions

## Project goal
Build a professional parking management application in C#.

## Architecture rules
- Do not put business logic inside WinForms/WPF forms.
- UI must call services only.
- Camera providers must implement IParkingCameraService.
- Card reader providers must implement ICardReaderDevice.
- Vehicle in/out logic must go through InOutBusinessService.
- Lane switching must go through LaneRouterService.
- License checks must go through LicenseGate.
- Storage must support offline-first local save and SyncOutbox.
- Do not call Dahua/Hikvision SDK directly from UI.
- Do not access SQL directly from UI.

## Business rules
- Unregistered card cannot enter.
- Disabled card cannot enter.
- Card already in parking cannot enter again.
- Vehicle not in parking cannot exit normally.
- Expired license blocks vehicle in.
- Expired license allows emergency vehicle out only.
- Vehicle in/out must create audit log.
- Vehicle in/out must create SyncOutbox event.

## Camera rules
- Use one interface: IParkingCameraService.
- Supported providers: Dahua, Hikvision, ONVIF.
- FullImage is used for storage.
- CenterCrop320x480 is used for UI and ANPR.
- Dahua must use JPEG callback first, fallback file capture.
- Hikvision must use NET_DVR_CaptureJPEGPicture first.
- ONVIF must use SnapshotUri first, fallback RTSP frame.

## License rules
- Gmail is account owner, not runtime key.
- MachineId + fingerprint required.
- Customer can activate only purchased number of machines.
- No auto-renew.
- Renewal requires signed confirmation code.
- Worker checks periodically but does not renew.
- Expired license:
  - no vehicle in
  - no data browsing
  - no report export
  - no config
  - no card registration
  - emergency vehicle out only

## Coding rules
- Add null checks.
- Dispose Bitmap/Image properly.
- Do not call GC.Collect manually.
- Do not concatenate SQL strings.
- Use repository/service classes for data access.
- Add tests for business logic.
- Keep each task small.

## Review focus
Report:
- Compile errors
- Business rule gaps
- SQL injection risk
- UI thread violations
- Bitmap memory leaks
- Missing transaction
- Missing audit log
- Device fallback missing
- License bypass
- Sync data loss risk
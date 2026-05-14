# DahuaCameraTestApp

App test Dahua doc lap de xac minh cach dung SDK Dahua truoc khi tich hop nguoc lai vao PPS/TP.Parking.New.

## Muc dich

- Dang nhap vao NVR Dahua bang IP/port/user/password.
- Mo live preview theo channel.
- Stop preview.
- Chup snapshot va luu file.
- Ghi log ro rang cho:
  - init SDK
  - login
  - preview
  - snapshot
  - logout / cleanup
  - ma loi SDK neu lay duoc

App nay khong phu thuoc Hangbang, khong can database, khong sua flow van hanh PPS chinh.

## Cau truc

- `DahuaCameraTestApp.sln`
- `DahuaCameraTestApp.csproj`
- `DahuaSdkClient.cs`
- `DahuaTestForm.cs`
- `libs/`

## Cach mo / chay

1. Mo:
   - `tools/DahuaCameraTestApp/DahuaCameraTestApp.sln`
   - hoac mo truc tiep `tools/DahuaCameraTestApp/DahuaCameraTestApp.csproj`
2. Build `Debug | x86`
3. Chay app

## Cau hinh mac dinh

Doc tu `App.config`:

- `DAHUA_IP = 192.168.1.142`
- `DAHUA_PORT = 37777`
- `DAHUA_USERNAME = admin`
- `DAHUA_PASSWORD = ""`
- `DAHUA_CHANNEL = 1`

Neu `DAHUA_PASSWORD` rong, app de o password trong cho nguoi dung nhap.

## DLL Dahua can co

App compile duoc ma khong can SDK. Tuy nhien de login/preview/snapshot thanh cong, can copy bo DLL Dahua vao mot trong hai noi:

1. Thu muc canh file exe
2. `tools/DahuaCameraTestApp/libs/` (neu chay local tu source thi copy vao day roi copy cung output khi can)

Toi thieu nen co:

- `DHNetSDKCS.dll`
- `dhnetsdk.dll`
- `dhconfigsdk.dll`
- `dhplay.dll`
- `dhlog.dll`

Tuy theo bo SDK, co the can them:

- `DHPlaySDKCS.dll`
- `DHVDCSDKCS.dll`

## Cach kiem tra ket qua

### Login thanh cong
- Bam `Login`
- Khu log hien:
  - init SDK ok
  - login success
  - login handle

### Preview co hinh
- Nhap `Channel` (mac dinh 1)
- Bam `Start Preview`
- Neu SDK va NVR ok, panel preview se co live image

### Snapshot tao duoc file
- Bam `Capture Snapshot`
- App se tao file vao:
  - `bin/Debug/DahuaSnapshots/`
  - hoac thu muc canh exe
- Khu log hien duong dan file thanh cong

## Loi thuong gap

### Thieu DLL SDK
- App van mo duoc
- Log hien ro:
  - khong tim thay `DHNetSDKCS.dll`
  - hoac load SDK that bai

### Sai port
- Login fail
- Kiem tra lai:
  - `37777`
  - firewall
  - NVR co mo SDK/API hay khong

### Sai user/password
- Login fail
- Kiem tra lai credential tren web/NVR

### Sai channel
- Login co the thanh cong nhung preview fail
- Thu channel `1`, `2`, `3`, `4`

### Firewall / network
- Ping/HTTP co the ok nhung SDK port chua chac mo
- Kiem tra routing, firewall Windows, firewall NVR

### NVR khong bat quyen SDK/API
- Login/preview fail du thong tin dung
- Kiem tra setting quyen truy cap tren NVR

## Anh huong den PPS chinh

- Khong anh huong build/runtime cua PPS/TP.Parking.New chinh
- Day la app test doc lap
- Muc dich chi de xac minh SDK Dahua truoc khi tich hop nguoc lai vao he thong chinh

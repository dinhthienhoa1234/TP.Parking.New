# DECISIONS.md - Các quyết định đã chốt trong dự án TP.Parking.New

## 1. Nguyên tắc chung

- Dự án ưu tiên ổn định vận hành hơn là làm mới toàn bộ.
- Không refactor lớn khi chưa cần.
- Không dùng thư viện ngoài nếu chưa được yêu cầu.
- Không tự ý đổi tên class/file/control/event handler.
- Ưu tiên giải pháp thực dụng, dễ build, dễ kiểm tra.

## 2. Giao diện xe vào/ra

Đã chốt:

- Dùng bố cục có thanh trên gồm XE VÀO / XE RA.
- Camera vào/ra cần đặt sát nhau để tận dụng tối đa chiều rộng.
- Dải vận hành cần full ngang.
- Panel trái/phải đổi màu theo làn vào/ra.
- Các ô Thông báo / Biển số / Tính tiền cần kéo sát nhau nhất có thể.
- Tiêu đề của các ô Thông báo / Biển số / Tính tiền cần gần textbox/control nhập liệu.
- Có thể tăng tối đa chiều rộng các ô camera nếu layout còn trống.
- Không nên bỏ viền hoàn toàn ở các panel/radio nếu làm giảm khả năng vận hành.
- Có thể làm giảm cảm giác background hoặc giả trong suốt, nhưng vẫn phải dễ nhìn và dễ thao tác.
- WinForms TextBox không hỗ trợ transparent thật, nên không cố ép transparent thật bằng cách gây lỗi paint/flicker.

## 3. Camera

Định hướng:

- Hệ thống hiện ưu tiên Dahua.
- Không để form gọi trực tiếp Dahua low-level nếu đã có service thay thế phù hợp.
- Định hướng dùng `CameraDeviceService.refactored.cs` để tách xử lý camera.

## 4. Đầu đọc thẻ / thiết bị vào ra

Định hướng:

- Hỗ trợ chọn loại đầu đọc:
  - SerialPort
  - CR208
  - CR501
- Định hướng dùng `InOutDeviceService.cs`.
- Không trộn quá nhiều xử lý thiết bị trực tiếp vào form chính.

## 5. Nghiệp vụ vào/ra

Định hướng:

- Dùng `InOutBusinessService.cs` cho nghiệp vụ vào/ra.
- Hạn chế để form chính chứa quá nhiều logic nghiệp vụ.
- Khi sửa giao diện, không làm thay đổi nghiệp vụ xử lý xe vào/ra.

## 6. SQL Server

Đã chốt:

- SQL Server 2019 Express.
- Ưu tiên script một chạm chạy trong SSMS.
- Script cần có kiểm tra lỗi và kết quả rõ ràng khi phù hợp.
- Một số bảng chính được ưu tiên giữ:
  - `Car`
  - `CarParkingSnapshot`
  - `CarArchive`
  - `VehicleTypeMaster`
  - `SystemPolicy`
  - `UserCar`
  - `Functional`
  - `TicketLog`
  - `CarOperationAudit`
  - `CarSearchAudit`
  - `fee.TariffProfile`
  - `fee.VehicleTariffProfileMap`
  - `fee.VehicleTariffRule`
  - `fee.SystemPolicy`
  - `fee.MonthlyPassSubscription`

## 7. Quy tắc khi dùng Codex/AI

- Không giao task quá rộng.
- Mỗi task chỉ nên có một mục tiêu chính.
- Luôn chỉ rõ file được phép sửa.
- Luôn chỉ rõ những thứ không được sửa.
- Sau mỗi PR, cần review phạm vi, build, runtime và rủi ro nghiệp vụ.

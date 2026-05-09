# PROJECT_CONTEXT.md - Bối cảnh dự án TP.Parking.New

## 1. Mục tiêu dự án

TP.Parking.New là dự án quản lý bãi xe thông minh, phục vụ vận hành thực tế tại bãi xe/chung cư/tòa nhà.

Mục tiêu ưu tiên:

- Ổn định khi vận hành.
- Dễ triển khai trên máy tính bãi xe.
- Dễ bảo trì.
- Hạn chế thay đổi lớn gây lỗi dây chuyền.
- Phù hợp với môi trường WinForms C# cũ và SQL Server Express.

## 2. Công nghệ chính

- WinForms C#
- Visual Studio 2022
- SQL Server 2019 Express
- Camera Dahua
- Đầu đọc thẻ có thể định hướng hỗ trợ:
  - SerialPort
  - CR208
  - CR501

## 3. Nguyên tắc kỹ thuật chung

- Ưu tiên sửa ít, chạy ổn.
- Không refactor lớn nếu chưa có yêu cầu rõ.
- Không đổi tên class/file/control tùy tiện.
- Không dùng thư viện ngoài nếu chưa được yêu cầu.
- Không làm thay đổi nghiệp vụ vào/ra khi task chỉ yêu cầu sửa giao diện.
- Không tự ý thay đổi database nếu task không liên quan SQL.
- Với giao diện giữ xe, ưu tiên vận hành rõ ràng, dễ nhìn, không làm đẹp quá mức gây khó dùng.

## 4. Các file quan trọng

### Giao diện

- `ucBikeInOut.PpsLayout.cs`
  - Layout giao diện xe vào/ra.
  - Đang được ưu tiên chỉnh giao diện vận hành.

### Form chính / nghiệp vụ

- `CameraSmartCardInOut.cs`
  - Form chính xử lý camera, thẻ, xe vào/ra.
  - Cần hạn chế sửa trực tiếp nếu task không yêu cầu.

### Camera

- `CameraDeviceService.refactored.cs`
  - Service xử lý thiết bị camera.
  - Định hướng dùng thay cho việc form gọi trực tiếp Dahua low-level.

### Thiết bị vào/ra

- `InOutDeviceService.cs`
  - Service xử lý thiết bị đọc thẻ/COM.
  - Định hướng hỗ trợ chọn loại đầu đọc: SerialPort, CR208, CR501.

### Nghiệp vụ vào/ra

- `InOutBusinessService.cs`
  - Service xử lý nghiệp vụ vào/ra.

### SQL

- `Homyplan3_Final.sql`
  - Script SQL nền tảng.
  - Dùng làm chuẩn khi rà database.

## 5. Quy tắc giao diện đã chốt

- Thanh trên có hai khu vực: XE VÀO / XE RA.
- Camera trái/phải sát nhau để tận dụng chiều rộng.
- Dải vận hành full ngang.
- Panel trái/phải đổi màu theo làn vào/ra.
- Các ô Thông báo / Biển số / Tính tiền cần gọn, rõ, ít cảm giác nền.
- Các panel/radio không nên bỏ viền hoàn toàn nếu làm giảm khả năng vận hành.
- WinForms TextBox không hỗ trợ transparent thật, nên ưu tiên giải pháp giả trong suốt, giảm nền hoặc dùng control thay thế hợp lý.
- Không đánh đổi sự rõ ràng vận hành chỉ để đạt hiệu ứng trong suốt.

## 6. Quy tắc khi làm việc với code cũ

- Không tự ý đổi namespace.
- Không tự ý đổi tên control.
- Không tự ý đổi event handler.
- Không tự ý xóa code nghiệp vụ cũ nếu chưa xác định chắc chắn không dùng.
- Nếu cần loại bỏ code cũ, phải nêu rõ:
  - Code nào bị loại bỏ.
  - Vì sao loại bỏ.
  - Có ảnh hưởng luồng vào/ra hay không.
  - Cách test lại.

## 7. Quy tắc khi test

Sau mỗi thay đổi code, cần ưu tiên test:

1. Build project bằng Visual Studio.
2. Mở form giao diện chính.
3. Kiểm tra không vỡ layout ở độ phân giải máy giữ xe.
4. Kiểm tra thao tác xe vào.
5. Kiểm tra thao tác xe ra.
6. Kiểm tra camera hiển thị.
7. Kiểm tra đọc thẻ.
8. Kiểm tra tính tiền.
9. Kiểm tra không phát sinh lỗi runtime khi mở form.

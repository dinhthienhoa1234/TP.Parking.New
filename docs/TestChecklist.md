# Test Checklist - TP.Parking.New

## 1. Build checklist
- [ ] Clean Solution
- [ ] Rebuild Solution
- [ ] 0 Error
- [ ] Không phát sinh file bin/obj/.vs trong commit

## 2. Runtime startup checklist
- [ ] Program.cs tạo ParkingAppBootstrapper
- [ ] BuildDefaultRuntime chạy không lỗi
- [ ] runtime.Start() chạy trước khi mở form
- [ ] Form nhận được ParkingAppRuntime
- [ ] Khi đóng app, runtime.Dispose() được gọi
- [ ] LicenseWorker start/stop không crash
- [ ] SyncWorker start/stop không crash

## 3. WinForms checklist
- [ ] App mở đúng startup project THPARKING.App.WinForms
- [ ] Form không crash khi mở
- [ ] Form title hiển thị runtime đã khởi tạo
- [ ] Form vẫn mở được bằng WinForms Designer
- [ ] UI không gọi SQL trực tiếp
- [ ] UI không gọi Dahua/Hikvision SDK trực tiếp
- [ ] UI không gọi SerialPort/CR208/CR501 trực tiếp

## 4. Business flow checklist
- [ ] Vehicle In đi qua ParkingAppOrchestrator
- [ ] Vehicle Out đi qua ParkingAppOrchestrator
- [ ] InOutBusinessService xử lý nghiệp vụ chính
- [ ] Thiếu mã thẻ trả lỗi rõ ràng
- [ ] Thiếu mã làn trả lỗi rõ ràng
- [ ] License hết hạn chặn xe vào
- [ ] License hết hạn chỉ cho emergency out theo rule
- [ ] Vehicle In tạo SyncOutbox event
- [ ] Vehicle Out tạo SyncOutbox event

## 5. Card reader checklist
- [ ] Tất cả đầu đọc phải implement ICardReaderDevice
- [ ] CardCodeService normalize mã thẻ
- [ ] CardCodeService chặn đọc trùng trong 1500ms
- [ ] UI chỉ nhận event card read qua service/device layer
- [ ] Không để legacy CR501/CR208 code lộ lên UI

## 6. Camera checklist
- [ ] Tất cả camera provider phải implement IParkingCameraService
- [ ] UI không gọi SDK camera trực tiếp
- [ ] Snapshot trả FullImage
- [ ] Snapshot trả CenterCrop320x480 hoặc ảnh crop tương đương
- [ ] Snapshot failure không làm crash app
- [ ] Camera service null vẫn không làm crash runtime

## 7. Sync checklist
- [ ] Offline-first: lưu local trước
- [ ] Tạo SyncOutbox event sau thao tác quan trọng
- [ ] Không chờ server trước khi trả UI
- [ ] Server offline không được chặn barrier nếu local commit đã thành công
- [ ] SyncWorker retry được event lỗi
- [ ] Sync failure không làm mất dữ liệu local

## 8. Database checklist
- [ ] Không nối chuỗi SQL trong code
- [ ] Dùng repository/service cho data access
- [ ] Vehicle In/Out phải chạy trong transaction khi có database thật
- [ ] AuditLog được tạo cho thao tác quan trọng
- [ ] Không xóa dữ liệu khi license hết hạn

## 9. Manual smoke test
- [ ] Mở app
- [ ] Kiểm tra title runtime
- [ ] Đóng app không treo process
- [ ] Mở lại app lần 2 không lỗi
- [ ] Build lại sau khi đóng app
- [ ] Không có exception trong Output

## 10. Pull Request checklist
- [ ] PR chỉ làm một mục tiêu
- [ ] Có Summary
- [ ] Có Changed
- [ ] Có Not included
- [ ] Có Verification
- [ ] Build local thành công trước khi tạo PR
- [ ] Không commit bin/obj/.vs
- [ ] Không sửa designer nếu PR không liên quan UI

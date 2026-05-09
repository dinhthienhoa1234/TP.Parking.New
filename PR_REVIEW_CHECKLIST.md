# PR_REVIEW_CHECKLIST.md - Checklist review PR cho TP.Parking.New

## 1. Thông tin PR

- PR số:
- Branch:
- Mục tiêu PR:
- File chính được sửa:

## 2. Kiểm tra phạm vi

- [ ] PR có sửa đúng file được yêu cầu không?
- [ ] Có sửa thêm file ngoài phạm vi không?
- [ ] Nếu có sửa thêm, lý do có hợp lý không?
- [ ] Có tự ý refactor lớn không?
- [ ] Có tự ý đổi namespace/class/control/event handler không?
- [ ] Có tự ý thêm thư viện ngoài không?

## 3. Kiểm tra WinForms

- [ ] Có sửa `.Designer.cs` không?
- [ ] Nếu có, có cần thiết không?
- [ ] Có nguy cơ lỗi Designer trong Visual Studio không?
- [ ] Layout có bị phụ thuộc cứng vào độ phân giải không?
- [ ] Có giữ được khả năng vận hành rõ ràng không?

## 4. Kiểm tra nghiệp vụ

- [ ] Có thay đổi luồng xe vào không?
- [ ] Có thay đổi luồng xe ra không?
- [ ] Có thay đổi xử lý camera không?
- [ ] Có thay đổi xử lý đọc thẻ không?
- [ ] Có thay đổi xử lý tính tiền không?
- [ ] Nếu PR chỉ sửa giao diện, có đảm bảo không đụng nghiệp vụ không?

## 5. Kiểm tra SQL nếu có

- [ ] Có script SQL không?
- [ ] Script có `BEGIN TRY / BEGIN CATCH` nếu cần không?
- [ ] Có transaction nếu sửa dữ liệu/cấu trúc quan trọng không?
- [ ] Có kiểm tra trước/sau không?
- [ ] Có drop bảng/cột/index không?
- [ ] Nếu có drop, đã được xác nhận rõ chưa?

## 6. Kiểm tra build/test

- [ ] Build project thành công.
- [ ] Không lỗi compile.
- [ ] Không lỗi runtime khi mở form.
- [ ] Không lỗi load camera.
- [ ] Không lỗi đọc thẻ.
- [ ] Không lỗi tính tiền.
- [ ] Không lỗi kết nối database.

## 7. Kết luận review

Chọn một:

- [ ] Có thể merge.
- [ ] Có thể merge nhưng cần lưu ý.
- [ ] Chưa nên merge, cần sửa tiếp.

## 8. Ghi chú rủi ro

Ghi rõ rủi ro còn lại:

- 
- 
- 

## 9. Checklist test sau merge

Sau khi merge, cần test:

1. Mở app.
2. Mở form xe vào/ra.
3. Test xe vào.
4. Test xe ra.
5. Test camera.
6. Test đọc thẻ.
7. Test tính tiền.
8. Kiểm tra log/lỗi runtime.

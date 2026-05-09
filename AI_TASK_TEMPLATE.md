# AI_TASK_TEMPLATE.md - Mẫu giao việc cho Codex/Claude/AI

## 1. Yêu cầu

[Viết yêu cầu cụ thể ở đây]

Ví dụ:

Sửa file `ucBikeInOut.PpsLayout.cs` để các ô Thông báo / Biển số / Tính tiền nhìn gọn hơn, ít cảm giác background hơn, nhưng vẫn giữ viền rõ để vận hành dễ.

## 2. Phạm vi được sửa

Chỉ được sửa các file sau:

- `[Tên file 1]`
- `[Tên file 2 nếu có]`

Không được sửa ngoài phạm vi nếu không có lý do compile/runtime rõ ràng.

## 3. Bối cảnh bắt buộc phải đọc

Trước khi làm, đọc:

- `AGENTS.md`
- `PROJECT_CONTEXT.md`
- `docs/DECISIONS.md`

## 4. Ràng buộc kỹ thuật

- Không refactor lan rộng.
- Không đổi namespace.
- Không đổi tên class.
- Không đổi tên control.
- Không đổi event handler nếu không cần.
- Không dùng thư viện ngoài.
- Không sửa database nếu task không yêu cầu SQL.
- Không sửa nghiệp vụ vào/ra nếu task chỉ yêu cầu giao diện.
- Với WinForms cũ, hạn chế đụng `.Designer.cs` nếu có thể xử lý bằng file layout/service riêng.

## 5. Kết quả mong muốn

Khi hoàn thành, trả về:

1. Tóm tắt đã sửa gì.
2. Danh sách file đã sửa.
3. Có sửa ngoài phạm vi không.
4. Rủi ro còn lại.
5. Checklist test nhanh.
6. Nếu có code cần copy thủ công, đưa nguyên file hoặc nguyên block đầy đủ.

## 6. Checklist test nhanh

Sau khi sửa, cần kiểm tra:

- Build project thành công.
- Không lỗi Designer.
- Mở được form chính.
- Layout không bị vỡ.
- Không đổi luồng nghiệp vụ vào/ra.
- Không ảnh hưởng camera/đọc thẻ/tính tiền nếu task không liên quan.

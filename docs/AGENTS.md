# AGENTS.md - Quy tắc làm việc cho AI/Codex trong dự án TP.Parking.New

## 1. Ngôn ngữ và cách trả lời

- Trả lời bằng tiếng Việt.
- Viết rõ ràng, thực dụng, ưu tiên hướng dẫn có thể làm ngay.
- Không giải thích lan man nếu người dùng đang cần code/script/bản dùng ngay.
- Nếu có rủi ro khi copy code, phải báo trước ngắn gọn.

## 2. Quy tắc khi sửa code/script

1. Khi sửa code/script, luôn ưu tiên dán nguyên bản hoàn chỉnh để người dùng copy trực tiếp.
2. Không mặc định tạo file tải về, trừ khi người dùng yêu cầu rõ.
3. Nếu file dài, chia thành nhiều phần nhưng vẫn phải là bản hoàn chỉnh, không phải đoạn vá rời.
4. Trước code phải tóm tắt ngắn gọn đã sửa gì và lưu ý khi dán.
5. Sau code phải có hướng dẫn kiểm tra nhanh.
6. Không lặp lại nội dung đã thống nhất hoặc đã hoàn thành.
7. Nếu thiếu thông tin nhưng vẫn có thể làm bản nháp hợp lý, cứ làm trước và ghi rõ giả định.

## 3. Quy tắc với WinForms C# cũ

- Ưu tiên giải pháp thực dụng, ít phá cấu trúc cũ.
- Không dùng thư viện ngoài nếu chưa được yêu cầu.
- Không tự ý đổi namespace, tên class, tên control, tên file nếu không cần thiết.
- Không refactor lan rộng nếu yêu cầu chỉ là sửa giao diện hoặc vá lỗi nhỏ.
- Không tự ý sửa file `.Designer.cs` nếu có thể xử lý bằng file layout/service riêng.
- Nếu bắt buộc sửa `.Designer.cs`, phải báo rõ lý do và rủi ro.
- Không dùng kỹ thuật quá mới nếu dự án đang chạy .NET Framework cũ.
- Ưu tiên code dễ build trên Visual Studio 2022.

## 4. Quy tắc với SQL Server

- Ưu tiên script một lần chạy được trong SSMS.
- Nếu script sửa dữ liệu/cấu trúc quan trọng, nên có `BEGIN TRY / BEGIN CATCH`.
- Nếu phù hợp, dùng transaction để có thể rollback khi lỗi.
- Có phần kiểm tra trước/sau nếu cần.
- Không drop bảng/cột/index nếu chưa được xác nhận rõ.
- Kết quả trả về phải dễ hiểu cho người vận hành không rành SQL.

## 5. Quy tắc với tài liệu/hợp đồng

- Viết bản sạch có thể copy dùng ngay.
- Văn phong doanh nghiệp, thực chiến, bảo vệ quyền lợi bên yêu cầu.
- Không viết quá giống văn AI.
- Không liệt kê khô cứng nếu người dùng yêu cầu văn bản gửi khách hàng/đối tác.

## 6. Quy tắc khi làm PR

Khi hoàn thành task hoặc PR, phải báo rõ:

1. Đã sửa file nào.
2. Mỗi file sửa nội dung gì.
3. Có sửa ngoài phạm vi yêu cầu hay không.
4. Có rủi ro compile/runtime nào không.
5. Cách test nhanh.
6. Có nên merge hay chưa.

## 7. Quy tắc giới hạn phạm vi

Nếu task chỉ yêu cầu sửa một file, chỉ sửa file đó.

Nếu cần sửa file khác để compile hoặc để logic chạy đúng, phải giải thích rõ:

- Vì sao phải sửa thêm.
- File nào bị ảnh hưởng.
- Mức độ rủi ro.
- Có cách nào không sửa thêm không.

## 8. Quy tắc tránh hao token/quota

- Không đọc/quét toàn bộ repo nếu task đã chỉ rõ file cần sửa.
- Không refactor lớn nếu không có yêu cầu.
- Không tự ý thêm tính năng ngoài yêu cầu.
- Không tự ý viết lại kiến trúc nếu chỉ cần vá lỗi.
- Không lặp lại phân tích đã có trong `PROJECT_CONTEXT.md` hoặc `DECISIONS.md`.

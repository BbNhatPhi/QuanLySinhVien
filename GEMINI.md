# QUY TẮC PHÁT TRIỂN & FONT TIẾNG VIỆT CHO DỰ ÁN QUẢN LÝ SINH VIÊN

## 1. BẮT BUỘC ĐỊNH DẠNG UTF-8 VỚI BOM (Byte Order Mark)
- **Tất cả các file** mã nguồn (`.cshtml`, `.cs`, `.css`, `.js`, `.sql`) khi tạo mới hoặc chỉnh sửa **BẮT BUỘC** phải được lưu bằng encoding **UTF-8 with BOM** (3 byte đầu: `0xEF, 0xBB, 0xBF`).
- Trình phân tích Razor của ASP.NET MVC trên Windows IIS Express mặc định đọc file không có BOM theo ANSI Code Page (Windows-1252), gây lỗi bể font tiếng Việt. Lưu UTF-8 with BOM giải quyết triệt để lỗi này.
- Lệnh PowerShell chuẩn:
  ```powershell
  $utf8WithBom = New-Object System.Text.UTF8Encoding($true)
  [System.IO.File]::WriteAllText($path, $content, $utf8WithBom)
  ```

## 2. TIÊU CHUẨN FONT TIẾNG VIỆT
- Toàn bộ giao diện sử dụng font chữ chính thức:
  `font-family: 'Be Vietnam Pro', 'Segoe UI', -apple-system, BlinkMacSystemFont, Roboto, sans-serif !important;`
- Google Font `Be Vietnam Pro` phải được import trong `Site.css` và thẻ `<head>` của các layout/view độc lập.
- Tất cả các nhãn, thông báo, nút bấm, bảng dữ liệu phải hiển thị tiếng Việt có dấu chuẩn xác và lịch sự theo phong cách cổng đào tạo đại học.

## 3. CÚ PHÁP RAZOR TRONG .CSHTML
- Mọi khối lệnh điều khiển `if`, `else if`, `else`, `foreach`, `for` trong `.cshtml` **bắt buộc phải có cặp ngoặc nhọn `{}`** bao quanh khối thân lệnh, không bao giờ viết dạng single-line không ngoặc vì Razor Engine sẽ báo `Parser Error`.

## 4. AN TOÀN HỆ THỐNG & CẤU HÌNH
- **Không bao giờ xóa** hoặc làm hỏng các file cấu hình dự án: `.sln`, `.csproj`, `App.config`, `Web.config`.
- Mọi form POST phải có `@Html.AntiForgeryToken()` và controller tương ứng phải có `[ValidateAntiForgeryToken]`.
- Sau mỗi lần chỉnh sửa file C# hoặc cấu hình, luôn chạy biên dịch MSBuild để đảm bảo 0 Errors, 0 Warnings trước khi hoàn thành.

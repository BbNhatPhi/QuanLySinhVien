-- ==========================================
-- 1. ADD NEW SUBJECTS INTO MonHoc
-- ==========================================

-- Kỹ thuật phần mềm (1) - Thêm môn
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'IT06') INSERT INTO MonHoc VALUES ('IT06', N'Mạng máy tính', 3);
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'IT07') INSERT INTO MonHoc VALUES ('IT07', N'Hệ điều hành', 3);
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'IT08') INSERT INTO MonHoc VALUES ('IT08', N'Kiểm thử phần mềm', 4);
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'IT09') INSERT INTO MonHoc VALUES ('IT09', N'Đồ án tốt nghiệp KTPM', 10);

-- Bác sĩ Thú y (2) - Thêm môn
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'VET02') INSERT INTO MonHoc VALUES ('VET02', N'Giải phẫu động vật', 4);
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'VET03') INSERT INTO MonHoc VALUES ('VET03', N'Ký sinh trùng thú y', 3);
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'VET04') INSERT INTO MonHoc VALUES ('VET04', N'Bệnh lý học thú y', 3);
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'VET05') INSERT INTO MonHoc VALUES ('VET05', N'Dược lý thú y', 4);

-- AVCD1 (3) - Tiếng Anh
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'ENG01') INSERT INTO MonHoc VALUES ('ENG01', N'Ngữ pháp cơ bản', 3);
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'ENG02') INSERT INTO MonHoc VALUES ('ENG02', N'Kỹ năng Nghe - Nói', 4);
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'ENG03') INSERT INTO MonHoc VALUES ('ENG03', N'Kỹ năng Đọc - Viết', 4);
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'ENG04') INSERT INTO MonHoc VALUES ('ENG04', N'Tiếng Anh chuyên ngành', 3);

-- Dự án cơ sở (4)
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'PRJ01') INSERT INTO MonHoc VALUES ('PRJ01', N'Quản lý dự án', 3);
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'PRJ02') INSERT INTO MonHoc VALUES ('PRJ02', N'Phương pháp nghiên cứu', 3);
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'PRJ03') INSERT INTO MonHoc VALUES ('PRJ03', N'Kỹ năng thuyết trình', 2);

-- Trí tuệ nhân tạo (5) - Thêm môn
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'AI05') INSERT INTO MonHoc VALUES ('AI05', N'Mạng nơ-ron nhân tạo', 3);
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'AI06') INSERT INTO MonHoc VALUES ('AI06', N'Học sâu (Deep Learning)', 4);
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'AI07') INSERT INTO MonHoc VALUES ('AI07', N'Khai phá dữ liệu', 3);
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'AI08') INSERT INTO MonHoc VALUES ('AI08', N'Đồ án tốt nghiệp AI', 10);

-- Chế biến thực phẩm (6) - Thêm môn
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'FOOD4') INSERT INTO MonHoc VALUES ('FOOD4', N'Đánh giá cảm quan', 3);
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'FOOD5') INSERT INTO MonHoc VALUES ('FOOD5', N'ATVS Thực phẩm', 3);
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'FOOD6') INSERT INTO MonHoc VALUES ('FOOD6', N'Công nghệ lên men', 4);

-- CSKH (7)
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'CSKH1') INSERT INTO MonHoc VALUES ('CSKH1', N'Tâm lý khách hàng', 3);
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'CSKH2') INSERT INTO MonHoc VALUES ('CSKH2', N'Kỹ năng giao tiếp', 3);
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'CSKH3') INSERT INTO MonHoc VALUES ('CSKH3', N'Quản trị CRM', 4);
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'CSKH4') INSERT INTO MonHoc VALUES ('CSKH4', N'Xử lý khiếu nại', 3);

-- TEST123 (8)
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'TST01') INSERT INTO MonHoc VALUES ('TST01', N'Môn Kiểm thử 1', 2);
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'TST02') INSERT INTO MonHoc VALUES ('TST02', N'Môn Kiểm thử 2', 2);

-- Kỹ thuật ô tô (9) - Thêm môn
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'AUTO4') INSERT INTO MonHoc VALUES ('AUTO4', N'Hệ thống truyền lực', 3);
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'AUTO5') INSERT INTO MonHoc VALUES ('AUTO5', N'Thiết kế ô tô', 4);
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'AUTO6') INSERT INTO MonHoc VALUES ('AUTO6', N'Chẩn đoán lỗi ô tô', 3);

-- ==========================================
-- 2. MAP NEW SUBJECTS TO ChuongTrinhKhung
-- ==========================================

-- Kỹ thuật phần mềm (1)
IF NOT EXISTS (SELECT * FROM ChuongTrinhKhung WHERE NganhID = 1 AND MaMon = 'IT06') INSERT INTO ChuongTrinhKhung VALUES (1, 'IT06', 5);
IF NOT EXISTS (SELECT * FROM ChuongTrinhKhung WHERE NganhID = 1 AND MaMon = 'IT07') INSERT INTO ChuongTrinhKhung VALUES (1, 'IT07', 5);
IF NOT EXISTS (SELECT * FROM ChuongTrinhKhung WHERE NganhID = 1 AND MaMon = 'IT08') INSERT INTO ChuongTrinhKhung VALUES (1, 'IT08', 6);
IF NOT EXISTS (SELECT * FROM ChuongTrinhKhung WHERE NganhID = 1 AND MaMon = 'IT09') INSERT INTO ChuongTrinhKhung VALUES (1, 'IT09', 8);

-- Bác sĩ Thú y (2)
IF NOT EXISTS (SELECT * FROM ChuongTrinhKhung WHERE NganhID = 2 AND MaMon = 'VET02') INSERT INTO ChuongTrinhKhung VALUES (2, 'VET02', 2);
IF NOT EXISTS (SELECT * FROM ChuongTrinhKhung WHERE NganhID = 2 AND MaMon = 'VET03') INSERT INTO ChuongTrinhKhung VALUES (2, 'VET03', 3);
IF NOT EXISTS (SELECT * FROM ChuongTrinhKhung WHERE NganhID = 2 AND MaMon = 'VET04') INSERT INTO ChuongTrinhKhung VALUES (2, 'VET04', 4);
IF NOT EXISTS (SELECT * FROM ChuongTrinhKhung WHERE NganhID = 2 AND MaMon = 'VET05') INSERT INTO ChuongTrinhKhung VALUES (2, 'VET05', 4);

-- AVCD1 (3)
IF NOT EXISTS (SELECT * FROM ChuongTrinhKhung WHERE NganhID = 3 AND MaMon = 'DC01') INSERT INTO ChuongTrinhKhung VALUES (3, 'DC01', 1);
IF NOT EXISTS (SELECT * FROM ChuongTrinhKhung WHERE NganhID = 3 AND MaMon = 'ENG01') INSERT INTO ChuongTrinhKhung VALUES (3, 'ENG01', 1);
IF NOT EXISTS (SELECT * FROM ChuongTrinhKhung WHERE NganhID = 3 AND MaMon = 'ENG02') INSERT INTO ChuongTrinhKhung VALUES (3, 'ENG02', 2);
IF NOT EXISTS (SELECT * FROM ChuongTrinhKhung WHERE NganhID = 3 AND MaMon = 'ENG03') INSERT INTO ChuongTrinhKhung VALUES (3, 'ENG03', 3);
IF NOT EXISTS (SELECT * FROM ChuongTrinhKhung WHERE NganhID = 3 AND MaMon = 'ENG04') INSERT INTO ChuongTrinhKhung VALUES (3, 'ENG04', 4);

-- Dự án cơ sở (4)
IF NOT EXISTS (SELECT * FROM ChuongTrinhKhung WHERE NganhID = 4 AND MaMon = 'DC01') INSERT INTO ChuongTrinhKhung VALUES (4, 'DC01', 1);
IF NOT EXISTS (SELECT * FROM ChuongTrinhKhung WHERE NganhID = 4 AND MaMon = 'PRJ01') INSERT INTO ChuongTrinhKhung VALUES (4, 'PRJ01', 2);
IF NOT EXISTS (SELECT * FROM ChuongTrinhKhung WHERE NganhID = 4 AND MaMon = 'PRJ02') INSERT INTO ChuongTrinhKhung VALUES (4, 'PRJ02', 3);
IF NOT EXISTS (SELECT * FROM ChuongTrinhKhung WHERE NganhID = 4 AND MaMon = 'PRJ03') INSERT INTO ChuongTrinhKhung VALUES (4, 'PRJ03', 4);

-- Trí tuệ nhân tạo (5)
IF NOT EXISTS (SELECT * FROM ChuongTrinhKhung WHERE NganhID = 5 AND MaMon = 'AI05') INSERT INTO ChuongTrinhKhung VALUES (5, 'AI05', 5);
IF NOT EXISTS (SELECT * FROM ChuongTrinhKhung WHERE NganhID = 5 AND MaMon = 'AI06') INSERT INTO ChuongTrinhKhung VALUES (5, 'AI06', 5);
IF NOT EXISTS (SELECT * FROM ChuongTrinhKhung WHERE NganhID = 5 AND MaMon = 'AI07') INSERT INTO ChuongTrinhKhung VALUES (5, 'AI07', 6);
IF NOT EXISTS (SELECT * FROM ChuongTrinhKhung WHERE NganhID = 5 AND MaMon = 'AI08') INSERT INTO ChuongTrinhKhung VALUES (5, 'AI08', 8);

-- Chế biến thực phẩm (6)
IF NOT EXISTS (SELECT * FROM ChuongTrinhKhung WHERE NganhID = 6 AND MaMon = 'FOOD4') INSERT INTO ChuongTrinhKhung VALUES (6, 'FOOD4', 5);
IF NOT EXISTS (SELECT * FROM ChuongTrinhKhung WHERE NganhID = 6 AND MaMon = 'FOOD5') INSERT INTO ChuongTrinhKhung VALUES (6, 'FOOD5', 5);
IF NOT EXISTS (SELECT * FROM ChuongTrinhKhung WHERE NganhID = 6 AND MaMon = 'FOOD6') INSERT INTO ChuongTrinhKhung VALUES (6, 'FOOD6', 6);

-- CSKH (7)
IF NOT EXISTS (SELECT * FROM ChuongTrinhKhung WHERE NganhID = 7 AND MaMon = 'DC01') INSERT INTO ChuongTrinhKhung VALUES (7, 'DC01', 1);
IF NOT EXISTS (SELECT * FROM ChuongTrinhKhung WHERE NganhID = 7 AND MaMon = 'CSKH1') INSERT INTO ChuongTrinhKhung VALUES (7, 'CSKH1', 1);
IF NOT EXISTS (SELECT * FROM ChuongTrinhKhung WHERE NganhID = 7 AND MaMon = 'CSKH2') INSERT INTO ChuongTrinhKhung VALUES (7, 'CSKH2', 2);
IF NOT EXISTS (SELECT * FROM ChuongTrinhKhung WHERE NganhID = 7 AND MaMon = 'CSKH3') INSERT INTO ChuongTrinhKhung VALUES (7, 'CSKH3', 3);
IF NOT EXISTS (SELECT * FROM ChuongTrinhKhung WHERE NganhID = 7 AND MaMon = 'CSKH4') INSERT INTO ChuongTrinhKhung VALUES (7, 'CSKH4', 4);

-- TEST123 (8)
IF NOT EXISTS (SELECT * FROM ChuongTrinhKhung WHERE NganhID = 8 AND MaMon = 'DC01') INSERT INTO ChuongTrinhKhung VALUES (8, 'DC01', 1);
IF NOT EXISTS (SELECT * FROM ChuongTrinhKhung WHERE NganhID = 8 AND MaMon = 'TST01') INSERT INTO ChuongTrinhKhung VALUES (8, 'TST01', 2);
IF NOT EXISTS (SELECT * FROM ChuongTrinhKhung WHERE NganhID = 8 AND MaMon = 'TST02') INSERT INTO ChuongTrinhKhung VALUES (8, 'TST02', 3);

-- Kỹ thuật ô tô (9)
IF NOT EXISTS (SELECT * FROM ChuongTrinhKhung WHERE NganhID = 9 AND MaMon = 'AUTO4') INSERT INTO ChuongTrinhKhung VALUES (9, 'AUTO4', 4);
IF NOT EXISTS (SELECT * FROM ChuongTrinhKhung WHERE NganhID = 9 AND MaMon = 'AUTO5') INSERT INTO ChuongTrinhKhung VALUES (9, 'AUTO5', 5);
IF NOT EXISTS (SELECT * FROM ChuongTrinhKhung WHERE NganhID = 9 AND MaMon = 'AUTO6') INSERT INTO ChuongTrinhKhung VALUES (9, 'AUTO6', 6);


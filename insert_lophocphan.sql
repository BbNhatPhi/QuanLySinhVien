-- ==========================================
-- TẠO LỚP HỌC PHẦN CHO CÁC NGÀNH KHÁC NHAU
-- ==========================================
DECLARE @HocKy INT = 1;
DECLARE @NamHoc VARCHAR(20) = '2026-2027';
DECLARE @TrangThai NVARCHAR(50) = N'Mở đăng ký';
DECLARE @SoLuongMax INT = 40;

-- 1. Kỹ thuật phần mềm (GV001, GV1)
IF NOT EXISTS (SELECT * FROM LopHocPhan WHERE MaLopHP = 'IT06_01') INSERT INTO LopHocPhan VALUES ('IT06_01', 'IT06', 'GV001', @HocKy, @NamHoc, @SoLuongMax, @TrangThai);
IF NOT EXISTS (SELECT * FROM LopHocPhan WHERE MaLopHP = 'IT07_01') INSERT INTO LopHocPhan VALUES ('IT07_01', 'IT07', 'GV1', @HocKy, @NamHoc, @SoLuongMax, @TrangThai);
IF NOT EXISTS (SELECT * FROM LopHocPhan WHERE MaLopHP = 'IT08_01') INSERT INTO LopHocPhan VALUES ('IT08_01', 'IT08', 'GV001', @HocKy, @NamHoc, @SoLuongMax, @TrangThai);
IF NOT EXISTS (SELECT * FROM LopHocPhan WHERE MaLopHP = 'IT09_01') INSERT INTO LopHocPhan VALUES ('IT09_01', 'IT09', 'GV1', @HocKy, @NamHoc, @SoLuongMax, @TrangThai);

-- 2. Bác sĩ thú y (GV002, GV36)
IF NOT EXISTS (SELECT * FROM LopHocPhan WHERE MaLopHP = 'VET02_01') INSERT INTO LopHocPhan VALUES ('VET02_01', 'VET02', 'GV002', @HocKy, @NamHoc, @SoLuongMax, @TrangThai);
IF NOT EXISTS (SELECT * FROM LopHocPhan WHERE MaLopHP = 'VET03_01') INSERT INTO LopHocPhan VALUES ('VET03_01', 'VET03', 'GV36', @HocKy, @NamHoc, @SoLuongMax, @TrangThai);
IF NOT EXISTS (SELECT * FROM LopHocPhan WHERE MaLopHP = 'VET04_01') INSERT INTO LopHocPhan VALUES ('VET04_01', 'VET04', 'GV002', @HocKy, @NamHoc, @SoLuongMax, @TrangThai);
IF NOT EXISTS (SELECT * FROM LopHocPhan WHERE MaLopHP = 'VET05_01') INSERT INTO LopHocPhan VALUES ('VET05_01', 'VET05', 'GV36', @HocKy, @NamHoc, @SoLuongMax, @TrangThai);

-- 3. AI / Trí tuệ nhân tạo (GV003, GV001)
IF NOT EXISTS (SELECT * FROM LopHocPhan WHERE MaLopHP = 'AI05_01') INSERT INTO LopHocPhan VALUES ('AI05_01', 'AI05', 'GV003', @HocKy, @NamHoc, @SoLuongMax, @TrangThai);
IF NOT EXISTS (SELECT * FROM LopHocPhan WHERE MaLopHP = 'AI06_01') INSERT INTO LopHocPhan VALUES ('AI06_01', 'AI06', 'GV001', @HocKy, @NamHoc, @SoLuongMax, @TrangThai);
IF NOT EXISTS (SELECT * FROM LopHocPhan WHERE MaLopHP = 'AI07_01') INSERT INTO LopHocPhan VALUES ('AI07_01', 'AI07', 'GV003', @HocKy, @NamHoc, @SoLuongMax, @TrangThai);
IF NOT EXISTS (SELECT * FROM LopHocPhan WHERE MaLopHP = 'AI08_01') INSERT INTO LopHocPhan VALUES ('AI08_01', 'AI08', 'GV001', @HocKy, @NamHoc, @SoLuongMax, @TrangThai);

-- 4. Thực phẩm (GV123)
IF NOT EXISTS (SELECT * FROM LopHocPhan WHERE MaLopHP = 'FOOD4_01') INSERT INTO LopHocPhan VALUES ('FOOD4_01', 'FOOD4', 'GV123', @HocKy, @NamHoc, @SoLuongMax, @TrangThai);
IF NOT EXISTS (SELECT * FROM LopHocPhan WHERE MaLopHP = 'FOOD5_01') INSERT INTO LopHocPhan VALUES ('FOOD5_01', 'FOOD5', 'GV123', @HocKy, @NamHoc, @SoLuongMax, @TrangThai);
IF NOT EXISTS (SELECT * FROM LopHocPhan WHERE MaLopHP = 'FOOD6_01') INSERT INTO LopHocPhan VALUES ('FOOD6_01', 'FOOD6', 'GV123', @HocKy, @NamHoc, @SoLuongMax, @TrangThai);

-- 5. Tiếng Anh / AVCD1 (GVtest, GV002)
IF NOT EXISTS (SELECT * FROM LopHocPhan WHERE MaLopHP = 'ENG01_01') INSERT INTO LopHocPhan VALUES ('ENG01_01', 'ENG01', 'GVtest', @HocKy, @NamHoc, @SoLuongMax, @TrangThai);
IF NOT EXISTS (SELECT * FROM LopHocPhan WHERE MaLopHP = 'ENG02_01') INSERT INTO LopHocPhan VALUES ('ENG02_01', 'ENG02', 'GV002', @HocKy, @NamHoc, @SoLuongMax, @TrangThai);
IF NOT EXISTS (SELECT * FROM LopHocPhan WHERE MaLopHP = 'ENG03_01') INSERT INTO LopHocPhan VALUES ('ENG03_01', 'ENG03', 'GVtest', @HocKy, @NamHoc, @SoLuongMax, @TrangThai);
IF NOT EXISTS (SELECT * FROM LopHocPhan WHERE MaLopHP = 'ENG04_01') INSERT INTO LopHocPhan VALUES ('ENG04_01', 'ENG04', 'GV002', @HocKy, @NamHoc, @SoLuongMax, @TrangThai);

-- 6. Dự án cơ sở (GV1, GV36)
IF NOT EXISTS (SELECT * FROM LopHocPhan WHERE MaLopHP = 'PRJ01_01') INSERT INTO LopHocPhan VALUES ('PRJ01_01', 'PRJ01', 'GV1', @HocKy, @NamHoc, @SoLuongMax, @TrangThai);
IF NOT EXISTS (SELECT * FROM LopHocPhan WHERE MaLopHP = 'PRJ02_01') INSERT INTO LopHocPhan VALUES ('PRJ02_01', 'PRJ02', 'GV36', @HocKy, @NamHoc, @SoLuongMax, @TrangThai);
IF NOT EXISTS (SELECT * FROM LopHocPhan WHERE MaLopHP = 'PRJ03_01') INSERT INTO LopHocPhan VALUES ('PRJ03_01', 'PRJ03', 'GV1', @HocKy, @NamHoc, @SoLuongMax, @TrangThai);

-- 7. Kỹ thuật ô tô (GV003, GV123)
IF NOT EXISTS (SELECT * FROM LopHocPhan WHERE MaLopHP = 'AUTO4_01') INSERT INTO LopHocPhan VALUES ('AUTO4_01', 'AUTO4', 'GV003', @HocKy, @NamHoc, @SoLuongMax, @TrangThai);
IF NOT EXISTS (SELECT * FROM LopHocPhan WHERE MaLopHP = 'AUTO5_01') INSERT INTO LopHocPhan VALUES ('AUTO5_01', 'AUTO5', 'GV123', @HocKy, @NamHoc, @SoLuongMax, @TrangThai);
IF NOT EXISTS (SELECT * FROM LopHocPhan WHERE MaLopHP = 'AUTO6_01') INSERT INTO LopHocPhan VALUES ('AUTO6_01', 'AUTO6', 'GV003', @HocKy, @NamHoc, @SoLuongMax, @TrangThai);

-- 8. CSKH (GV002, GVtest)
IF NOT EXISTS (SELECT * FROM LopHocPhan WHERE MaLopHP = 'CSKH1_01') INSERT INTO LopHocPhan VALUES ('CSKH1_01', 'CSKH1', 'GV002', @HocKy, @NamHoc, @SoLuongMax, @TrangThai);
IF NOT EXISTS (SELECT * FROM LopHocPhan WHERE MaLopHP = 'CSKH2_01') INSERT INTO LopHocPhan VALUES ('CSKH2_01', 'CSKH2', 'GVtest', @HocKy, @NamHoc, @SoLuongMax, @TrangThai);
IF NOT EXISTS (SELECT * FROM LopHocPhan WHERE MaLopHP = 'CSKH3_01') INSERT INTO LopHocPhan VALUES ('CSKH3_01', 'CSKH3', 'GV002', @HocKy, @NamHoc, @SoLuongMax, @TrangThai);


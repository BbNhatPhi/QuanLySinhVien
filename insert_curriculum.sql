-- Insert more Subjects into MonHoc
-- Kỹ thuật phần mềm (1)
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'IT01') INSERT INTO MonHoc VALUES ('IT01', N'Nhập môn Lập trình', 3);
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'IT02') INSERT INTO MonHoc VALUES ('IT02', N'Cấu trúc dữ liệu và Giải thuật', 3);
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'IT03') INSERT INTO MonHoc VALUES ('IT03', N'Cơ sở dữ liệu', 3);
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'IT04') INSERT INTO MonHoc VALUES ('IT04', N'Thiết kế web', 3);
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'IT05') INSERT INTO MonHoc VALUES ('IT05', N'Kiến trúc phần mềm', 4);

-- Trí tuệ nhân tạo (5)
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'AI01') INSERT INTO MonHoc VALUES ('AI01', N'Nhập môn Trí tuệ nhân tạo', 3);
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'AI02') INSERT INTO MonHoc VALUES ('AI02', N'Học máy (Machine Learning)', 4);
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'AI03') INSERT INTO MonHoc VALUES ('AI03', N'Thị giác máy tính (Computer Vision)', 3);
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'AI04') INSERT INTO MonHoc VALUES ('AI04', N'Xử lý ngôn ngữ tự nhiên', 3);

-- Chế biến thực phẩm (6)
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'FOOD1') INSERT INTO MonHoc VALUES ('FOOD1', N'Hóa sinh thực phẩm', 3);
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'FOOD2') INSERT INTO MonHoc VALUES ('FOOD2', N'Vi sinh vật học thực phẩm', 3);
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'FOOD3') INSERT INTO MonHoc VALUES ('FOOD3', N'Công nghệ bảo quản', 3);

-- Kỹ thuật ô tô (9)
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'AUTO1') INSERT INTO MonHoc VALUES ('AUTO1', N'Cấu tạo ô tô', 3);
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'AUTO2') INSERT INTO MonHoc VALUES ('AUTO2', N'Động cơ đốt trong', 4);
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'AUTO3') INSERT INTO MonHoc VALUES ('AUTO3', N'Hệ thống điện ô tô', 3);

-- Môn đại cương chung cho tất cả
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'DC01') INSERT INTO MonHoc VALUES ('DC01', N'Triết học Mác - Lênin', 3);
IF NOT EXISTS (SELECT * FROM MonHoc WHERE MaMon = 'DC02') INSERT INTO MonHoc VALUES ('DC02', N'Tiếng Anh cơ bản', 4);

-- CLEAR Existing ChuongTrinhKhung to avoid duplicates and re-insert cleanly
DELETE FROM ChuongTrinhKhung;

-- MAP TO ChuongTrinhKhung
-- Nganh 1 (Kỹ thuật phần mềm)
INSERT INTO ChuongTrinhKhung (NganhID, MaMon, HocKyTieuChuan) VALUES 
(1, 'DC01', 1), (1, 'DC02', 1), (1, 'IT01', 1),
(1, 'IT02', 2), (1, 'IT03', 2),
(1, 'IT04', 3), (1, 'PRJ301', 3),
(1, 'IT05', 4), (1, 'CNTT02', 4);

-- Nganh 5 (Trí tuệ nhân tạo)
INSERT INTO ChuongTrinhKhung (NganhID, MaMon, HocKyTieuChuan) VALUES 
(5, 'DC01', 1), (5, 'DC02', 1), (5, 'IT01', 1),
(5, 'AI01', 2), (5, 'IT02', 2),
(5, 'AI02', 3), (5, 'AI03', 3),
(5, 'AI04', 4);

-- Nganh 6 (Chế biến thực phẩm)
INSERT INTO ChuongTrinhKhung (NganhID, MaMon, HocKyTieuChuan) VALUES 
(6, 'DC01', 1), (6, 'DC02', 1),
(6, 'FOOD1', 2),
(6, 'FOOD2', 3),
(6, 'FOOD3', 4);

-- Nganh 9 (Kỹ thuật ô tô)
INSERT INTO ChuongTrinhKhung (NganhID, MaMon, HocKyTieuChuan) VALUES 
(9, 'DC01', 1), (9, 'DC02', 1),
(9, 'AUTO1', 2),
(9, 'AUTO2', 3),
(9, 'AUTO3', 4),
(9, 'KTOT', 5);

-- Nganh 2 (Bác sĩ Thú y)
INSERT INTO ChuongTrinhKhung (NganhID, MaMon, HocKyTieuChuan) VALUES 
(2, 'DC01', 1), (2, 'DC02', 1),
(2, 'VET101', 2),
(2, 'DDK24', 3);


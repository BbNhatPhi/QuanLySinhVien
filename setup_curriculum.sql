ALTER TABLE SinhVien ADD NganhID INT;
GO
UPDATE SinhVien SET NganhID = 1 WHERE KhoaID = 1;
UPDATE SinhVien SET NganhID = 2 WHERE KhoaID = 2;
UPDATE SinhVien SET NganhID = 3 WHERE KhoaID = 3;
UPDATE SinhVien SET NganhID = 6 WHERE KhoaID = 8;
UPDATE SinhVien SET NganhID = 9 WHERE KhoaID = 11;
UPDATE SinhVien SET NganhID = 1 WHERE NganhID IS NULL;

-- Create ChuongTrinhKhung
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ChuongTrinhKhung')
BEGIN
    CREATE TABLE ChuongTrinhKhung (
        NganhID INT FOREIGN KEY REFERENCES Nganh(NganhID),
        MaMon VARCHAR(20) FOREIGN KEY REFERENCES MonHoc(MaMon),
        HocKyTieuChuan INT,
        PRIMARY KEY (NganhID, MaMon)
    );
END

-- Insert some curriculum data
INSERT INTO ChuongTrinhKhung (NganhID, MaMon, HocKyTieuChuan) VALUES 
(1, 'CNTT02', 1),
(1, 'PRJ301', 2),
(1, 'TEST', 3),
(1, 'TEST2', 4),
(2, 'VET101', 1),
(2, 'DDK24', 2),
(9, 'KTOT', 1);

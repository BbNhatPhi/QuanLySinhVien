-- =======================================================
-- PHẦN 1: TẠO CƠ SỞ DỮ LIỆU
-- =======================================================
IF DB_ID('StudentManagementSystem') IS NOT NULL
BEGIN
    USE master;
    ALTER DATABASE StudentManagementSystem SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE StudentManagementSystem;
END
GO

CREATE DATABASE StudentManagementSystem;
GO

USE StudentManagementSystem;
GO

-- =======================================================
-- PHẦN 2: TẠO CÁC BẢNG (TABLES)
-- =======================================================

-- 1. Phân quyền & Người dùng
CREATE TABLE Roles (
    RoleID INT PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL
);

CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Username VARCHAR(50) UNIQUE NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    RoleID INT NOT NULL,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
);

-- 2. Đơn vị đào tạo
CREATE TABLE Khoa (
    KhoaID INT IDENTITY(1,1) PRIMARY KEY,
    TenKhoa NVARCHAR(100) NOT NULL,
    LienHe NVARCHAR(100)
);

CREATE TABLE Nganh (
    NganhID INT IDENTITY(1,1) PRIMARY KEY,
    TenNganh NVARCHAR(100) NOT NULL,
    KhoaID INT NOT NULL,
    FOREIGN KEY (KhoaID) REFERENCES Khoa(KhoaID)
);

CREATE TABLE MonHoc (
    MaMon VARCHAR(20) PRIMARY KEY,
    TenMon NVARCHAR(100) NOT NULL,
    SoTinChi INT NOT NULL
);

-- 3. Hồ sơ nhân sự & Sinh viên
CREATE TABLE AdminProfile (
    AdminID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT UNIQUE NOT NULL,
    HoTen NVARCHAR(100) NOT NULL,
    Email VARCHAR(100),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

CREATE TABLE NhanVien (
    MaNV VARCHAR(20) PRIMARY KEY,
    UserID INT UNIQUE NOT NULL,
    HoTen NVARCHAR(100) NOT NULL,
    PhongBan NVARCHAR(100),
    Email VARCHAR(100),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

CREATE TABLE GiangVien (
    MaGV VARCHAR(20) PRIMARY KEY,
    UserID INT UNIQUE NOT NULL,
    HoTen NVARCHAR(100) NOT NULL,
    KhoaID INT NOT NULL,
    Email VARCHAR(100),
    SoDienThoai VARCHAR(15),
    MaMon VARCHAR(20),
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (KhoaID) REFERENCES Khoa(KhoaID),
    FOREIGN KEY (MaMon) REFERENCES MonHoc(MaMon)
);

CREATE TABLE SinhVien (
    MaSV VARCHAR(20) PRIMARY KEY,
    UserID INT UNIQUE NOT NULL,
    HoTen NVARCHAR(100) NOT NULL,
    NgaySinh DATE,
    GioiTinh NVARCHAR(10),
    DiaChi NVARCHAR(255),
    Email VARCHAR(100),
    SoDienThoai VARCHAR(15),
    NganhID INT NOT NULL,
    TrangThaiHocTap NVARCHAR(50) DEFAULT N'Đang học',
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (NganhID) REFERENCES Nganh(NganhID)
);

-- 4. Quản lý Đào tạo 
CREATE TABLE LopDanhNghia (
    MaLop VARCHAR(20) PRIMARY KEY,
    TenLop NVARCHAR(100) NOT NULL,
    NganhID INT NOT NULL,
    KhoaHoc VARCHAR(20),
    FOREIGN KEY (NganhID) REFERENCES Nganh(NganhID)
);

CREATE TABLE LopHocPhan (
    MaLopHP VARCHAR(20) PRIMARY KEY,
    MaMon VARCHAR(20) NOT NULL,
    MaGV VARCHAR(20) NOT NULL,
    HocKy INT NOT NULL,
    NamHoc VARCHAR(20) NOT NULL,
    SoLuongMax INT NOT NULL,
    TrangThai NVARCHAR(50) DEFAULT N'Mở đăng ký',
    FOREIGN KEY (MaMon) REFERENCES MonHoc(MaMon),
    FOREIGN KEY (MaGV) REFERENCES GiangVien(MaGV)
);

CREATE TABLE DangKyHocPhan (
    MaDK INT IDENTITY(1,1) PRIMARY KEY,
    MaSV VARCHAR(20) NOT NULL,
    MaLopHP VARCHAR(20) NOT NULL,
    NgayDK DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (MaSV) REFERENCES SinhVien(MaSV),
    FOREIGN KEY (MaLopHP) REFERENCES LopHocPhan(MaLopHP),
    UNIQUE (MaSV, MaLopHP)
);

CREATE TABLE Diem (
    MaDiem INT IDENTITY(1,1) PRIMARY KEY,
    MaSV VARCHAR(20) NOT NULL,
    MaLopHP VARCHAR(20) NOT NULL,
    DiemCC FLOAT DEFAULT 10, 
    DiemGK FLOAT DEFAULT 0,
    DiemCK FLOAT DEFAULT 0,
    DiemTong FLOAT,
    FOREIGN KEY (MaSV) REFERENCES SinhVien(MaSV),
    FOREIGN KEY (MaLopHP) REFERENCES LopHocPhan(MaLopHP),
    UNIQUE (MaSV, MaLopHP)
);

CREATE TABLE ThoiKhoaBieu (
    MaTKB INT IDENTITY(1,1) PRIMARY KEY,
    MaLopHP VARCHAR(20) NOT NULL,
    NgayHoc INT NOT NULL, 
    CaHoc INT NOT NULL,   
    PhongHoc VARCHAR(50) NOT NULL,
    FOREIGN KEY (MaLopHP) REFERENCES LopHocPhan(MaLopHP)
);

CREATE TABLE HocPhi (
    MaHocPhi INT IDENTITY(1,1) PRIMARY KEY,
    MaSV VARCHAR(20) NOT NULL,
    HocKy INT NOT NULL,
    NamHoc VARCHAR(20) NOT NULL,
    TongTien DECIMAL(18,2) NOT NULL,
    DaNop DECIMAL(18,2) DEFAULT 0,
    NgayHan DATETIME,
    TrangThai NVARCHAR(50) DEFAULT N'Chưa hoàn thành',
    FOREIGN KEY (MaSV) REFERENCES SinhVien(MaSV)
);

CREATE TABLE ThongBao (
    MaTB INT IDENTITY(1,1) PRIMARY KEY,
    TieuDe NVARCHAR(255) NOT NULL,
    NoiDung NVARCHAR(MAX) NOT NULL,
    NgayDang DATETIME DEFAULT GETDATE(),
    NguoiDang NVARCHAR(100) DEFAULT N'Phòng Đào Tạo',
    DoiTuong NVARCHAR(50) DEFAULT 'All' 
);
GO

-- =======================================================
-- PHẦN 3: INSERT DỮ LIỆU MẪU (ĐÃ ĐỔI TÊN ĐĂNG NHẬP NGẮN GỌN)
-- =======================================================
INSERT INTO Roles (RoleID, RoleName) VALUES 
(1, 'Admin'), (2, 'GiangVien'), (3, 'NhanVien'), (4, 'SinhVien');

-- Mật khẩu tất cả đều là 123456
INSERT INTO Users (Username, PasswordHash, RoleID) VALUES 
('admin', '123456', 1),
('giangvien', '123456', 2),
('nhanvien', '123456', 3),
('sinhvien', '123456', 4);

INSERT INTO Khoa (TenKhoa, LienHe) VALUES 
(N'Công nghệ Thông tin', 'cntt@truong.edu.vn'),
(N'Thú y', 'thuy@truong.edu.vn');

INSERT INTO Nganh (TenNganh, KhoaID) VALUES 
(N'Kỹ thuật phần mềm', 1),
(N'Bác sĩ Thú y', 2);

INSERT INTO MonHoc (MaMon, TenMon, SoTinChi) VALUES 
('PRJ301', N'Lập trình C# .NET', 3),
('VET101', N'Mô phôi học Thú y', 3);

INSERT INTO AdminProfile (UserID, HoTen, Email) VALUES 
(1, N'Quản trị viên Hệ thống', 'admin@truong.edu.vn');

INSERT INTO GiangVien (MaGV, UserID, HoTen, KhoaID, Email, SoDienThoai, MaMon) VALUES 
('GV001', 2, N'Nguyễn Văn A', 1, 'nguyenvana@truong.edu.vn', '0901234567', 'PRJ301');

INSERT INTO NhanVien (MaNV, UserID, HoTen, PhongBan, Email) VALUES 
('NV001', 3, N'Trần Thị B', N'Phòng Đào tạo', 'tranthib@truong.edu.vn');

INSERT INTO SinhVien (MaSV, UserID, HoTen, NgaySinh, GioiTinh, DiaChi, Email, SoDienThoai, NganhID) VALUES 
('230001', 4, N'Lê Minh C', '2004-05-15', N'Nam', N'TP.HCM', '230001@student.edu.vn', '0987654321', 1);

INSERT INTO LopDanhNghia (MaLop, TenLop, NganhID, KhoaHoc) VALUES 
('23DTH1', N'Lớp Kỹ thuật PM K23', 1, 'K23');

INSERT INTO LopHocPhan (MaLopHP, MaMon, MaGV, HocKy, NamHoc, SoLuongMax) VALUES 
('LHP_PRJ301_01', 'PRJ301', 'GV001', 1, '2026-2027', 40);

INSERT INTO ThoiKhoaBieu (MaLopHP, NgayHoc, CaHoc, PhongHoc) VALUES 
('LHP_PRJ301_01', 3, 1, 'A1-202');

INSERT INTO ThongBao (TieuDe, NoiDung, DoiTuong) VALUES 
(N'Thông báo nộp học phí Học kỳ 1 (2026-2027)', N'Yêu cầu hoàn thành việc đóng học phí trước 30/09.', 'SinhVien');
GO

-- =======================================================
-- PHẦN 4: KHỞI TẠO STORED PROCEDURES 
-- =======================================================

-- ================= ADMIN & USER SPs =================
CREATE OR ALTER PROCEDURE sp_GetDashboardStats AS
BEGIN
    SELECT 
        (SELECT COUNT(*) FROM SinhVien) AS TotalStudents,
        (SELECT COUNT(*) FROM GiangVien) AS TotalTeachers,
        (SELECT COUNT(*) FROM Khoa) AS TotalKhoa,
        (SELECT COUNT(*) FROM Users WHERE IsActive = 1) AS ActiveUsers;
END
GO

CREATE OR ALTER PROCEDURE sp_GetAllUsersList AS
BEGIN
    SELECT u.UserID, u.Username, u.RoleID, u.IsActive, r.RoleName,
           COALESCE(sv.HoTen, gv.HoTen, nv.HoTen, a.HoTen, N'Chưa cập nhật') AS HoTen,
           COALESCE(sv.Email, gv.Email, nv.Email, a.Email, '') AS Email
    FROM Users u
    INNER JOIN Roles r ON u.RoleID = r.RoleID
    LEFT JOIN SinhVien sv ON u.UserID = sv.UserID
    LEFT JOIN GiangVien gv ON u.UserID = gv.UserID
    LEFT JOIN NhanVien nv ON u.UserID = nv.UserID
    LEFT JOIN AdminProfile a ON u.UserID = a.UserID
    ORDER BY u.UserID DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_CreateUser @Username VARCHAR(50), @PasswordHash VARCHAR(255), @RoleID INT AS
BEGIN
    INSERT INTO Users (Username, PasswordHash, RoleID) VALUES (@Username, @PasswordHash, @RoleID);
END
GO

CREATE OR ALTER PROCEDURE sp_ToggleUserStatus @UserID INT, @IsActive BIT AS
BEGIN
    UPDATE Users SET IsActive = @IsActive WHERE UserID = @UserID;
END
GO

CREATE OR ALTER PROCEDURE sp_UpdateUserRole @UserID INT, @RoleID INT AS
BEGIN
    UPDATE Users SET RoleID = @RoleID WHERE UserID = @UserID;
END
GO

CREATE OR ALTER PROCEDURE sp_BackupDatabase @BackupPath NVARCHAR(255) OUTPUT AS
BEGIN
    BEGIN TRY
        DECLARE @FileName NVARCHAR(255) = 'C:\Backup_SMS\SMS_Backup_' + FORMAT(GETDATE(), 'yyyyMMdd_HHmmss') + '.bak';
        BACKUP DATABASE StudentManagementSystem TO DISK = @FileName WITH FORMAT, MEDIANAME = 'SMS_Backups', NAME = 'Full Backup of SMS';
        SET @BackupPath = @FileName;
    END TRY
    BEGIN CATCH
        SET @BackupPath = 'ERROR: ' + ERROR_MESSAGE();
    END CATCH
END
GO

-- ================= HỒ SƠ & SINH VIÊN SPs =================
CREATE OR ALTER PROCEDURE sp_GetThongTinCaNhan @Username VARCHAR(50) AS
BEGIN
    SELECT sv.MaSV, sv.HoTen, sv.NgaySinh, sv.GioiTinh, sv.Email, sv.SoDienThoai, sv.DiaChi
    FROM SinhVien sv INNER JOIN Users u ON sv.UserID = u.UserID WHERE u.Username = @Username;
END
GO

CREATE OR ALTER PROCEDURE sp_UpdateThongTinCaNhan @MaSV VARCHAR(20), @Email VARCHAR(100), @SoDienThoai VARCHAR(15), @DiaChi NVARCHAR(255), @Message NVARCHAR(255) OUTPUT AS
BEGIN
    BEGIN TRY
        UPDATE SinhVien SET Email = @Email, SoDienThoai = @SoDienThoai, DiaChi = @DiaChi WHERE MaSV = @MaSV;
        SET @Message = 'Success';
    END TRY
    BEGIN CATCH
        SET @Message = ERROR_MESSAGE();
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE sp_GetAllSinhVien AS
BEGIN
    SELECT sv.MaSV, sv.HoTen, sv.NgaySinh, sv.GioiTinh, sv.DiaChi, sv.Email, sv.SoDienThoai, sv.TrangThaiHocTap, n.TenNganh
    FROM SinhVien sv INNER JOIN Nganh n ON sv.NganhID = n.NganhID ORDER BY sv.MaSV DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_AddSinhVien @MaSV VARCHAR(20), @HoTen NVARCHAR(100), @NgaySinh DATE, @GioiTinh NVARCHAR(10), @Email VARCHAR(100), @NganhID INT, @Message NVARCHAR(255) OUTPUT AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @UserID INT;
        SELECT @UserID = UserID FROM Users WHERE Username = @MaSV;
        IF @UserID IS NULL BEGIN
            INSERT INTO Users (Username, PasswordHash, RoleID) VALUES (@MaSV, '123456', 4);
            SET @UserID = SCOPE_IDENTITY();
        END
        INSERT INTO SinhVien (MaSV, UserID, HoTen, NgaySinh, GioiTinh, Email, NganhID, TrangThaiHocTap) 
        VALUES (@MaSV, @UserID, @HoTen, @NgaySinh, @GioiTinh, @Email, ISNULL(@NganhID, 1), N'Đang học');
        COMMIT TRANSACTION;
        SET @Message = 'Success';
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SET @Message = ERROR_MESSAGE();
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE sp_DeleteSinhVien @MaSV VARCHAR(20) AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @UserID INT;
        SELECT @UserID = UserID FROM SinhVien WHERE MaSV = @MaSV;
        DELETE FROM SinhVien WHERE MaSV = @MaSV;
        IF @UserID IS NOT NULL DELETE FROM Users WHERE UserID = @UserID;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- ================= GIẢNG VIÊN SPs =================
CREATE OR ALTER PROCEDURE sp_GetDanhSachGiangVien AS
BEGIN
    SELECT gv.MaGV, gv.HoTen, gv.Email, gv.SoDienThoai, k.KhoaID, k.TenKhoa, gv.MaMon, m.TenMon
    FROM GiangVien gv LEFT JOIN Khoa k ON gv.KhoaID = k.KhoaID LEFT JOIN MonHoc m ON gv.MaMon = m.MaMon ORDER BY gv.MaGV DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_GetAllGiangVien AS
BEGIN
    SELECT MaGV, HoTen FROM GiangVien;
END
GO

CREATE OR ALTER PROCEDURE sp_GetLichDayGiangVien @Username VARCHAR(50) AS
BEGIN
    SELECT lhp.MaLopHP, m.TenMon, tkb.PhongHoc, tkb.NgayHoc, tkb.CaHoc
    FROM Users u INNER JOIN GiangVien gv ON u.UserID = gv.UserID INNER JOIN LopHocPhan lhp ON gv.MaGV = lhp.MaGV
    INNER JOIN ThoiKhoaBieu tkb ON lhp.MaLopHP = tkb.MaLopHP INNER JOIN MonHoc m ON lhp.MaMon = m.MaMon
    WHERE u.Username = @Username ORDER BY tkb.NgayHoc ASC, tkb.CaHoc ASC;
END
GO

CREATE OR ALTER PROCEDURE sp_AddGiangVien @MaGV VARCHAR(20), @HoTen NVARCHAR(100), @KhoaID INT, @Email VARCHAR(100), @SoDienThoai VARCHAR(15), @MaMon VARCHAR(20), @Message NVARCHAR(255) OUTPUT AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @UserID INT;
        SELECT @UserID = UserID FROM Users WHERE Username = @MaGV;
        IF @UserID IS NULL BEGIN
            INSERT INTO Users (Username, PasswordHash, RoleID) VALUES (@MaGV, '123456', 2);
            SET @UserID = SCOPE_IDENTITY();
        END
        INSERT INTO GiangVien (MaGV, UserID, HoTen, KhoaID, Email, SoDienThoai, MaMon) 
        VALUES (@MaGV, @UserID, @HoTen, @KhoaID, @Email, @SoDienThoai, @MaMon);
        COMMIT TRANSACTION;
        SET @Message = 'Success';
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        IF ERROR_NUMBER() = 2627 SET @Message = N'Mã Giảng viên này đã tồn tại!';
        ELSE SET @Message = ERROR_MESSAGE();
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE sp_DeleteGiangVien @MaGV VARCHAR(20) AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @UserID INT;
        SELECT @UserID = UserID FROM GiangVien WHERE MaGV = @MaGV;
        DELETE FROM GiangVien WHERE MaGV = @MaGV;
        IF @UserID IS NOT NULL DELETE FROM Users WHERE UserID = @UserID;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- ================= ĐÀO TẠO & LỚP SPs =================
CREATE OR ALTER PROCEDURE sp_GetAllKhoaList AS
BEGIN
    SELECT KhoaID, TenKhoa, LienHe FROM Khoa;
END
GO

CREATE OR ALTER PROCEDURE sp_AddKhoaAdmin @TenKhoa NVARCHAR(100), @LienHe NVARCHAR(100), @Message NVARCHAR(255) OUTPUT AS
BEGIN
    BEGIN TRY
        IF EXISTS (SELECT 1 FROM Khoa WHERE TenKhoa = @TenKhoa) BEGIN SET @Message = N'Tên khoa này đã tồn tại!'; RETURN; END
        INSERT INTO Khoa (TenKhoa, LienHe) VALUES (@TenKhoa, @LienHe);
        SET @Message = 'Success';
    END TRY
    BEGIN CATCH SET @Message = ERROR_MESSAGE(); END CATCH
END
GO

CREATE OR ALTER PROCEDURE sp_DeleteKhoa @KhoaID INT AS
BEGIN
    DELETE FROM Khoa WHERE KhoaID = @KhoaID;
END
GO

CREATE OR ALTER PROCEDURE sp_GetAllNganhList AS
BEGIN
    SELECT n.NganhID, n.TenNganh, n.KhoaID, k.TenKhoa FROM Nganh n INNER JOIN Khoa k ON n.KhoaID = k.KhoaID ORDER BY n.NganhID DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_AddNganhAdmin @TenNganh NVARCHAR(100), @KhoaID INT AS
BEGIN
    INSERT INTO Nganh (TenNganh, KhoaID) VALUES (@TenNganh, @KhoaID);
END
GO

CREATE OR ALTER PROCEDURE sp_GetAllMonHoc AS
BEGIN
    SELECT MaMon, TenMon, SoTinChi FROM MonHoc ORDER BY MaMon;
END
GO

CREATE OR ALTER PROCEDURE sp_AddMonHoc @MaMon VARCHAR(20), @TenMon NVARCHAR(100), @SoTinChi INT AS
BEGIN
    INSERT INTO MonHoc (MaMon, TenMon, SoTinChi) VALUES (@MaMon, @TenMon, @SoTinChi);
END
GO

CREATE OR ALTER PROCEDURE sp_GetAllLop AS
BEGIN
    SELECT l.MaLop, l.TenLop, l.KhoaHoc, l.NganhID, n.TenNganh FROM LopDanhNghia l INNER JOIN Nganh n ON l.NganhID = n.NganhID ORDER BY l.KhoaHoc DESC, l.MaLop ASC;
END
GO

CREATE OR ALTER PROCEDURE sp_AddLop @MaLop VARCHAR(20), @TenLop NVARCHAR(100), @NganhID INT, @KhoaHoc VARCHAR(20) AS
BEGIN
    INSERT INTO LopDanhNghia (MaLop, TenLop, NganhID, KhoaHoc) VALUES (@MaLop, @TenLop, @NganhID, @KhoaHoc);
END
GO

CREATE OR ALTER PROCEDURE sp_GetAllLopHocPhan AS
BEGIN
    SELECT lhp.MaLopHP, lhp.MaMon, m.TenMon, lhp.MaGV, gv.HoTen AS TenGV, lhp.HocKy, lhp.NamHoc, lhp.SoLuongMax, lhp.TrangThai
    FROM LopHocPhan lhp INNER JOIN MonHoc m ON lhp.MaMon = m.MaMon INNER JOIN GiangVien gv ON lhp.MaGV = gv.MaGV ORDER BY lhp.NamHoc DESC, lhp.HocKy DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_AddLopHocPhan @MaLopHP VARCHAR(20), @MaMon VARCHAR(20), @MaGV VARCHAR(20), @HocKy INT, @NamHoc VARCHAR(20), @SoLuongMax INT, @Message NVARCHAR(255) OUTPUT AS
BEGIN
    BEGIN TRY
        INSERT INTO LopHocPhan (MaLopHP, MaMon, MaGV, HocKy, NamHoc, SoLuongMax, TrangThai) VALUES (@MaLopHP, @MaMon, @MaGV, @HocKy, @NamHoc, @SoLuongMax, N'Mở đăng ký');
        SET @Message = 'Success';
    END TRY
    BEGIN CATCH
        IF ERROR_NUMBER() = 2627 SET @Message = N'Mã Lớp Học Phần này đã tồn tại!';
        ELSE IF ERROR_NUMBER() = 547 SET @Message = N'Lỗi: Mã Môn học hoặc Mã Giảng viên không tồn tại trong hệ thống!';
        ELSE SET @Message = ERROR_MESSAGE();
    END CATCH
END
GO

-- ================= ĐĂNG KÝ HỌC & HOẠT ĐỘNG KHÁC =================
CREATE OR ALTER PROCEDURE sp_SinhVienDangKyHoc @Username VARCHAR(50), @MaLopHP VARCHAR(20), @Message NVARCHAR(255) OUTPUT AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @MaSV VARCHAR(20);
        SELECT @MaSV = MaSV FROM SinhVien sv INNER JOIN Users u ON sv.UserID = u.UserID WHERE u.Username = @Username;
        IF @MaSV IS NULL BEGIN SET @Message = N'Không tìm thấy hồ sơ sinh viên của bạn!'; ROLLBACK TRANSACTION; RETURN; END

        DECLARE @SiSoHienTai INT, @SiSoMax INT;
        SELECT @SiSoHienTai = COUNT(*) FROM DangKyHocPhan WHERE MaLopHP = @MaLopHP;
        SELECT @SiSoMax = SoLuongMax FROM LopHocPhan WHERE MaLopHP = @MaLopHP;
        IF @SiSoHienTai >= @SiSoMax BEGIN SET @Message = N'Rất tiếc! Lớp học phần này đã hết chỗ.'; ROLLBACK TRANSACTION; RETURN; END

        INSERT INTO DangKyHocPhan (MaSV, MaLopHP) VALUES (@MaSV, @MaLopHP);
        INSERT INTO Diem (MaSV, MaLopHP, DiemCC, DiemGK, DiemCK, DiemTong) VALUES (@MaSV, @MaLopHP, 10, 0, 0, 0);

        COMMIT TRANSACTION;
        SET @Message = 'Success';
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        IF ERROR_NUMBER() = 2627 SET @Message = N'Bạn đã đăng ký lớp học phần này rồi!';
        ELSE SET @Message = ERROR_MESSAGE();
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE sp_GetAllTKB AS
BEGIN
    SELECT tkb.MaTKB, tkb.MaLopHP, m.TenMon, gv.HoTen AS TenGV, tkb.NgayHoc, tkb.CaHoc, tkb.PhongHoc
    FROM ThoiKhoaBieu tkb INNER JOIN LopHocPhan lhp ON tkb.MaLopHP = lhp.MaLopHP
    INNER JOIN MonHoc m ON lhp.MaMon = m.MaMon INNER JOIN GiangVien gv ON lhp.MaGV = gv.MaGV ORDER BY tkb.NgayHoc ASC, tkb.CaHoc ASC;
END
GO

CREATE OR ALTER PROCEDURE sp_AddTKB @MaLopHP VARCHAR(20), @NgayHoc INT, @CaHoc INT, @PhongHoc VARCHAR(50), @Message NVARCHAR(255) OUTPUT AS
BEGIN
    BEGIN TRY
        IF EXISTS (SELECT 1 FROM ThoiKhoaBieu WHERE NgayHoc = @NgayHoc AND CaHoc = @CaHoc AND PhongHoc = @PhongHoc)
        BEGIN SET @Message = N'Phòng học này đã có lớp sử dụng vào thời gian này!'; RETURN; END
        
        IF EXISTS (SELECT 1 FROM ThoiKhoaBieu tkb INNER JOIN LopHocPhan lhp ON tkb.MaLopHP = lhp.MaLopHP WHERE tkb.NgayHoc = @NgayHoc AND tkb.CaHoc = @CaHoc AND lhp.MaGV = (SELECT MaGV FROM LopHocPhan WHERE MaLopHP = @MaLopHP))
        BEGIN SET @Message = N'Giảng viên phụ trách lớp này đã bị trùng lịch dạy!'; RETURN; END

        INSERT INTO ThoiKhoaBieu (MaLopHP, NgayHoc, CaHoc, PhongHoc) VALUES (@MaLopHP, @NgayHoc, @CaHoc, @PhongHoc);
        SET @Message = 'Success';
    END TRY
    BEGIN CATCH SET @Message = ERROR_MESSAGE(); END CATCH
END
GO

CREATE OR ALTER PROCEDURE sp_GetHocPhiSinhVien @Username VARCHAR(50) AS
BEGIN
    DECLARE @DonGia FLOAT = 850000;
    SELECT lhp.MaLopHP, m.TenMon, m.SoTinChi, @DonGia AS DonGia
    FROM DangKyHocPhan dk INNER JOIN LopHocPhan lhp ON dk.MaLopHP = lhp.MaLopHP INNER JOIN MonHoc m ON lhp.MaMon = m.MaMon
    INNER JOIN SinhVien sv ON dk.MaSV = sv.MaSV INNER JOIN Users u ON sv.UserID = u.UserID
    WHERE u.Username = @Username ORDER BY lhp.NamHoc DESC, lhp.HocKy DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_GetThongBaoSinhVien AS
BEGIN
    SELECT MaTB, TieuDe, NoiDung, NgayDang, NguoiDang FROM ThongBao WHERE DoiTuong IN ('All', 'SinhVien') ORDER BY NgayDang DESC;
END
GO

PRINT N'Tái cấu trúc và tạo mới thành công toàn bộ Database SMS!';
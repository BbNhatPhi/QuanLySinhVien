-- =================================================================
-- HỆ THỐNG QUẢN LÝ SINH VIÊN (StudentManagementSystem)
-- BẢN CẬP NHẬT DATABASE & STORED PROCEDURES
-- =================================================================

USE [StudentManagementSystem];
GO

-- =================================================================
-- 1. STORED PROCEDURE: sp_HuyDangKyHocPhan
-- Nghiệp vụ: Sinh viên rút / hủy môn học đã đăng ký trong kỳ
-- =================================================================
CREATE OR ALTER PROCEDURE sp_HuyDangKyHocPhan
    @Username VARCHAR(50),
    @MaLopHP VARCHAR(20),
    @Message NVARCHAR(255) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. Xác định Mã sinh viên
        DECLARE @MaSV VARCHAR(20);
        SELECT @MaSV = sv.MaSV 
        FROM SinhVien sv 
        INNER JOIN Users u ON sv.UserID = u.UserID 
        WHERE u.Username = @Username;

        IF @MaSV IS NULL
        BEGIN
            SET @Message = N'Lỗi: Không tìm thấy hồ sơ sinh viên tương ứng với tài khoản này!';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 2. Kiểm tra xem sinh viên có đăng ký lớp này hay không
        IF NOT EXISTS (SELECT 1 FROM DangKyHocPhan WHERE MaSV = @MaSV AND MaLopHP = @MaLopHP)
        BEGIN
            SET @Message = N'Lỗi: Bạn chưa từng đăng ký lớp học phần này!';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 3. Kiểm tra xem lớp học phần còn trong thời gian cho phép (Trạng thái còn Mở đăng ký)
        DECLARE @TrangThaiLop NVARCHAR(50);
        SELECT @TrangThaiLop = TrangThai FROM LopHocPhan WHERE MaLopHP = @MaLopHP;

        IF @TrangThaiLop IS NULL OR @TrangThaiLop <> N'Mở đăng ký'
        BEGIN
            SET @Message = N'Lỗi: Lớp học phần này hiện đã khóa hoặc đã bắt đầu học, không thể hủy!';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 4. Kiểm tra trạng thái đóng học phí (nếu đã thanh toán = 1 thì không cho tự ý hủy trên web)
        DECLARE @TrangThaiThanhToan BIT;
        SELECT @TrangThaiThanhToan = TrangThaiThanhToan 
        FROM DangKyHocPhan 
        WHERE MaSV = @MaSV AND MaLopHP = @MaLopHP;

        IF @TrangThaiThanhToan = 1
        BEGIN
            SET @Message = N'Lỗi: Học phần này bạn đã thanh toán học phí. Vui lòng liên hệ trực tiếp Phòng Đào Tạo để rút học phần và hoàn phí!';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 5. Kiểm tra xem sinh viên đã có điểm quá trình (giữa kỳ / cuối kỳ) chưa
        IF EXISTS (
            SELECT 1 FROM Diem 
            WHERE MaSV = @MaSV AND MaLopHP = @MaLopHP AND (DiemGK > 0 OR DiemCK > 0)
        )
        BEGIN
            SET @Message = N'Lỗi: Học phần này đã có điểm quá trình, không thể hủy!';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 6. Xóa bản ghi điểm khởi tạo
        DELETE FROM Diem 
        WHERE MaSV = @MaSV AND MaLopHP = @MaLopHP;

        -- 7. Xóa bản ghi đăng ký học phần
        DELETE FROM DangKyHocPhan 
        WHERE MaSV = @MaSV AND MaLopHP = @MaLopHP;

        COMMIT TRANSACTION;
        SET @Message = 'Success';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SET @Message = N'Lỗi hệ thống: ' + ERROR_MESSAGE();
    END CATCH
END;
GO

-- =================================================================
-- 2. STORED PROCEDURE: sp_UpdateSinhVien
-- Nghiệp vụ: Cán bộ Phòng Đào Tạo chỉnh sửa thông tin hồ sơ sinh viên
-- =================================================================
CREATE OR ALTER PROCEDURE sp_UpdateSinhVien
    @MaSV VARCHAR(50),
    @HoTen NVARCHAR(100),
    @NgaySinh DATE = NULL,
    @GioiTinh NVARCHAR(10) = NULL,
    @Email VARCHAR(100) = NULL,
    @SoDienThoai VARCHAR(20) = NULL,
    @DiaChi NVARCHAR(255) = NULL,
    @KhoaID INT = NULL,
    @TrangThaiHocTap NVARCHAR(50) = NULL,
    @Message NVARCHAR(255) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS (SELECT 1 FROM SinhVien WHERE MaSV = @MaSV)
        BEGIN
            SET @Message = N'Lỗi: Không tìm thấy sinh viên có mã ' + @MaSV;
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Cập nhật thông tin trong bảng SinhVien
        UPDATE SinhVien
        SET HoTen = @HoTen,
            NgaySinh = @NgaySinh,
            GioiTinh = @GioiTinh,
            Email = @Email,
            SoDienThoai = @SoDienThoai,
            DiaChi = @DiaChi,
            KhoaID = @KhoaID,
            TrangThaiHocTap = ISNULL(@TrangThaiHocTap, TrangThaiHocTap)
        WHERE MaSV = @MaSV;

        -- Đồng bộ họ tên sang bảng Users tương ứng
        UPDATE Users
        SET HoTen = @HoTen
        WHERE UserID = (SELECT UserID FROM SinhVien WHERE MaSV = @MaSV);

        COMMIT TRANSACTION;
        SET @Message = 'Success';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SET @Message = N'Lỗi hệ thống: ' + ERROR_MESSAGE();
    END CATCH
END;
GO

-- =================================================================
-- 3. STORED PROCEDURE: sp_AddSinhVien (Cập nhật chuẩn hóa Unicode & thông tin liên hệ)
-- =================================================================
CREATE OR ALTER PROCEDURE sp_AddSinhVien
    @MaSV VARCHAR(50),
    @HoTen NVARCHAR(100),
    @NgaySinh DATE = NULL,
    @GioiTinh NVARCHAR(10) = NULL,
    @Email VARCHAR(100) = NULL,
    @KhoaID INT = NULL,
    @SoDienThoai VARCHAR(20) = NULL,
    @DiaChi NVARCHAR(255) = NULL,
    @Message NVARCHAR(255) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @NewUserID INT;

        -- 1. Kiểm tra mã sinh viên đã tồn tại chưa
        IF EXISTS (SELECT 1 FROM SinhVien WHERE MaSV = @MaSV)
        BEGIN
            SET @Message = N'Lỗi: Mã sinh viên ' + @MaSV + N' đã tồn tại trong hệ thống!';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- 2. Xử lý tài khoản người dùng
        IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = @MaSV)
        BEGIN
            DECLARE @DefaultPass VARCHAR(255);
            SELECT TOP 1 @DefaultPass = PasswordHash FROM Users WHERE RoleID = 4;
            IF @DefaultPass IS NULL SET @DefaultPass = '$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy'; -- 123456 hash

            INSERT INTO Users (Username, PasswordHash, RoleID, HoTen, IsActive)
            VALUES (@MaSV, @DefaultPass, 4, @HoTen, 1);
            
            SET @NewUserID = SCOPE_IDENTITY(); 
        END
        ELSE
        BEGIN
            SELECT @NewUserID = UserID FROM Users WHERE Username = @MaSV;
        END

        -- 3. Lưu hồ sơ sinh viên với chuẩn tiếng Việt có dấu
        INSERT INTO SinhVien (MaSV, HoTen, NgaySinh, GioiTinh, Email, SoDienThoai, DiaChi, KhoaID, TrangThaiHocTap, UserID)
        VALUES (@MaSV, @HoTen, @NgaySinh, @GioiTinh, @Email, @SoDienThoai, @DiaChi, @KhoaID, N'Đang học', @NewUserID); 

        -- 4. Khởi tạo điểm rèn luyện mặc định 100 điểm (Xuất sắc)
        IF NOT EXISTS (SELECT 1 FROM DiemRenLuyen WHERE MaSV = @MaSV AND HocKy = N'HK1 (2025-2026)')
        BEGIN
            INSERT INTO DiemRenLuyen (MaSV, HocKy, Diem, XepLoai)
            VALUES (@MaSV, N'HK1 (2025-2026)', 100, N'Xuất sắc');
        END

        COMMIT TRANSACTION;
        SET @Message = 'Success';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SET @Message = N'Lỗi: ' + ERROR_MESSAGE();
    END CATCH
END;
GO


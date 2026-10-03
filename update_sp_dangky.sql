ALTER PROCEDURE [dbo].[sp_SinhVienDangKyHoc]
    @Username VARCHAR(50),
    @MaLopHP VARCHAR(20),
    @Message NVARCHAR(255) OUTPUT
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Xác định Mã SV dựa vào tài khoản đăng nhập
        DECLARE @MaSV VARCHAR(20);
        SELECT @MaSV = MaSV FROM SinhVien sv INNER JOIN Users u ON sv.UserID = u.UserID WHERE u.Username = @Username;

        IF @MaSV IS NULL 
        BEGIN 
            SET @Message = N'Lỗi: Không tìm thấy hồ sơ sinh viên của bạn trong hệ thống!'; 
            ROLLBACK TRANSACTION; RETURN; 
        END

        -- Lấy thông tin Lớp học phần đang muốn đăng ký
        DECLARE @MaMon VARCHAR(20), @HocKy INT, @NamHoc VARCHAR(20), @SoTinChi INT;
        SELECT @MaMon = lhp.MaMon, @HocKy = lhp.HocKy, @NamHoc = lhp.NamHoc, @SoTinChi = m.SoTinChi
        FROM LopHocPhan lhp INNER JOIN MonHoc m ON lhp.MaMon = m.MaMon
        WHERE lhp.MaLopHP = @MaLopHP;

        -- =======================================
        -- TẦNG 1: KIỂM TRA SĨ SỐ LỚP
        -- =======================================
        DECLARE @SiSoHienTai INT, @SiSoMax INT;
        SELECT @SiSoHienTai = COUNT(*) FROM DangKyHocPhan WHERE MaLopHP = @MaLopHP;
        SELECT @SiSoMax = SoLuongMax FROM LopHocPhan WHERE MaLopHP = @MaLopHP;
        IF @SiSoHienTai >= @SiSoMax 
        BEGIN 
            SET @Message = N'Rất tiếc! Lớp học phần này đã hết chỗ.'; 
            ROLLBACK TRANSACTION; RETURN; 
        END

        -- =======================================
        -- TẦNG 2: KIỂM TRA MÔN TRÙNG LẶP (ĐÃ ĐĂNG KÝ TRONG KỲ)
        -- =======================================
        IF EXISTS (
            SELECT 1 FROM DangKyHocPhan dk
            INNER JOIN LopHocPhan lhp_cu ON dk.MaLopHP = lhp_cu.MaLopHP
            WHERE dk.MaSV = @MaSV AND lhp_cu.MaMon = @MaMon AND lhp_cu.HocKy = @HocKy AND lhp_cu.NamHoc = @NamHoc
        )
        BEGIN
            SET @Message = N'Lỗi: Bạn đã đăng ký một lớp khác của môn này trong học kỳ hiện tại rồi!';
            ROLLBACK TRANSACTION; RETURN; 
        END

        -- =======================================
        -- TẦNG 3: KIỂM TRA MÔN TIÊN QUYẾT
        -- =======================================
        IF EXISTS (
            SELECT 1 FROM MonTienQuyet mtq
            WHERE mtq.MaMon = @MaMon
            AND NOT EXISTS (
                SELECT 1 FROM Diem d 
                INNER JOIN LopHocPhan lhp_cu ON d.MaLopHP = lhp_cu.MaLopHP
                WHERE d.MaSV = @MaSV AND lhp_cu.MaMon = mtq.MaMonTQ AND d.DiemTong >= 4.0
            )
        )
        BEGIN
            SET @Message = N'Lỗi: Bạn chưa hoàn thành môn học tiên quyết để được phép học môn này!';
            ROLLBACK TRANSACTION; RETURN; 
        END

        -- =======================================
        -- TẦNG 4: KIỂM TRA TRÙNG THỜI KHÓA BIỂU
        -- =======================================
        IF EXISTS (
            SELECT 1 FROM ThoiKhoaBieu tkb_moi
            INNER JOIN ThoiKhoaBieu tkb_cu ON tkb_moi.NgayHoc = tkb_cu.NgayHoc AND tkb_moi.CaHoc = tkb_cu.CaHoc
            INNER JOIN DangKyHocPhan dk_cu ON tkb_cu.MaLopHP = dk_cu.MaLopHP
            WHERE tkb_moi.MaLopHP = @MaLopHP AND dk_cu.MaSV = @MaSV
        )
        BEGIN
            SET @Message = N'Lỗi: Lịch học của lớp này bị trùng ngày/ca với một môn khác bạn đã đăng ký!';
            ROLLBACK TRANSACTION; RETURN; 
        END

        -- =======================================
        -- TẦNG 5: KIỂM TRA GIỚI HẠN TÍN CHỈ (MAX 25)
        -- =======================================
        DECLARE @TongTinChiHienTai INT;
        SELECT @TongTinChiHienTai = ISNULL(SUM(m_cu.SoTinChi), 0)
        FROM DangKyHocPhan dk_cu
        INNER JOIN LopHocPhan lhp_cu ON dk_cu.MaLopHP = lhp_cu.MaLopHP
        INNER JOIN MonHoc m_cu ON lhp_cu.MaMon = m_cu.MaMon
        WHERE dk_cu.MaSV = @MaSV AND lhp_cu.HocKy = @HocKy AND lhp_cu.NamHoc = @NamHoc;

        IF (@TongTinChiHienTai + @SoTinChi) > 25
        BEGIN
            SET @Message = N'Lỗi: Đăng ký lớp này sẽ làm bạn vượt quá giới hạn 25 tín chỉ trong học kỳ!';
            ROLLBACK TRANSACTION; RETURN; 
        END

        -- =======================================
        -- NẾU VƯỢT QUA MỌI ĐIỀU KIỆN -> LƯU DỮ LIỆU
        -- =======================================
        INSERT INTO DangKyHocPhan (MaSV, MaLopHP, TrangThaiThanhToan) VALUES (@MaSV, @MaLopHP, 0);
        
        -- Khởi tạo bảng điểm trắng cho SV nếu chưa có
        IF NOT EXISTS (SELECT 1 FROM Diem WHERE MaSV = @MaSV AND MaLopHP = @MaLopHP)
        BEGIN
            INSERT INTO Diem (MaSV, MaLopHP, DiemCC, DiemGK, DiemCK, DiemTong) VALUES (@MaSV, @MaLopHP, 10, 0, 0, 0);
        END

        COMMIT TRANSACTION;
        SET @Message = 'Success';
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        IF ERROR_NUMBER() = 2627 SET @Message = N'Bạn đã đăng ký lớp học phần này rồi!';
        ELSE SET @Message = ERROR_MESSAGE();
    END CATCH
END

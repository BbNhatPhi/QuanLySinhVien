ALTER PROCEDURE [dbo].[sp_AddLichThi]
    @MaLopHP VARCHAR(20),
    @NgayThi DATE,
    @CaThi INT,
    @PhongThi VARCHAR(50),
    @Message NVARCHAR(255) OUTPUT
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. Kiểm tra Lớp học phần đã có lịch thi chưa?
        IF EXISTS (SELECT 1 FROM LichThi WHERE MaLopHP = @MaLopHP)
        BEGIN
            SET @Message = N'Lỗi: Lớp học phần này đã được xếp lịch thi trước đó!';
            ROLLBACK TRANSACTION; RETURN;
        END

        -- 2. Kiểm tra trùng phòng thi
        IF EXISTS (SELECT 1 FROM LichThi WHERE NgayThi = @NgayThi AND CaThi = @CaThi AND PhongThi = @PhongThi)
        BEGIN
            SET @Message = N'Lỗi: Phòng thi này đã có lớp khác sử dụng vào ca/ngày này!';
            ROLLBACK TRANSACTION; RETURN;
        END

        -- Lấy Giảng viên của Lớp học phần này
        DECLARE @MaGV VARCHAR(20);
        SELECT @MaGV = MaGV FROM LopHocPhan WHERE MaLopHP = @MaLopHP;

        -- 3. Kiểm tra Giảng viên có bị trùng lịch gác thi không (do dạy 2 lớp xếp thi cùng giờ)
        IF EXISTS (
            SELECT 1 FROM LichThi lt
            INNER JOIN LopHocPhan lhp ON lt.MaLopHP = lhp.MaLopHP
            WHERE lt.NgayThi = @NgayThi AND lt.CaThi = @CaThi AND lhp.MaGV = @MaGV
        )
        BEGIN
            SET @Message = N'Lỗi: Giảng viên của lớp này đã bị kẹt lịch gác thi một lớp khác vào cùng ca/ngày!';
            ROLLBACK TRANSACTION; RETURN;
        END

        -- 4. Kiểm tra xem có Sinh viên nào bị trùng lịch thi không
        -- Có sinh viên nào trong lớp này đang đăng ký một lớp khác cũng thi vào cùng ngày, cùng ca?
        DECLARE @TrungSV INT;
        SELECT @TrungSV = COUNT(DISTINCT dk_moi.MaSV)
        FROM DangKyHocPhan dk_moi
        WHERE dk_moi.MaLopHP = @MaLopHP
        AND EXISTS (
            SELECT 1 FROM DangKyHocPhan dk_cu
            INNER JOIN LichThi lt_cu ON dk_cu.MaLopHP = lt_cu.MaLopHP
            WHERE dk_cu.MaSV = dk_moi.MaSV
            AND lt_cu.NgayThi = @NgayThi AND lt_cu.CaThi = @CaThi
        );

        IF @TrungSV > 0
        BEGIN
            SET @Message = N'Lỗi: Phát hiện ' + CAST(@TrungSV AS NVARCHAR(10)) + N' sinh viên bị TRÙNG LỊCH THI với một môn khác. Vui lòng xếp ca/ngày khác!';
            ROLLBACK TRANSACTION; RETURN;
        END

        -- NẾU QUA HẾT THÌ THÊM LỊCH THI
        INSERT INTO LichThi (MaLopHP, NgayThi, CaThi, PhongThi) 
        VALUES (@MaLopHP, @NgayThi, @CaThi, @PhongThi);
        
        COMMIT TRANSACTION;
        SET @Message = 'Success';
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SET @Message = ERROR_MESSAGE();
    END CATCH
END

ALTER PROCEDURE [dbo].[sp_UpdateLichThi]
    @MaLopHP VARCHAR(20),
    @NgayThi DATE,
    @CaThi INT,
    @PhongThi VARCHAR(50),
    @Message NVARCHAR(255) OUTPUT
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. Kiểm tra trùng phòng (bỏ qua chính lớp này)
        IF EXISTS (
            SELECT 1 FROM LichThi 
            WHERE NgayThi = @NgayThi AND CaThi = @CaThi AND PhongThi = @PhongThi AND MaLopHP != @MaLopHP
        )
        BEGIN
            SET @Message = N'Lỗi: Trùng lịch! Phòng thi này đã được xếp cho một lớp khác vào cùng ca/ngày.';
            ROLLBACK TRANSACTION; RETURN;
        END

        -- Lấy Giảng viên của Lớp học phần này
        DECLARE @MaGV VARCHAR(20);
        SELECT @MaGV = MaGV FROM LopHocPhan WHERE MaLopHP = @MaLopHP;

        -- 2. Kiểm tra Giảng viên trùng ca (bỏ qua chính lớp này)
        IF EXISTS (
            SELECT 1 FROM LichThi lt
            INNER JOIN LopHocPhan lhp ON lt.MaLopHP = lhp.MaLopHP
            WHERE lt.NgayThi = @NgayThi AND lt.CaThi = @CaThi AND lhp.MaGV = @MaGV AND lt.MaLopHP != @MaLopHP
        )
        BEGIN
            SET @Message = N'Lỗi: Giảng viên của lớp này đã bị kẹt lịch gác thi một lớp khác vào cùng ca/ngày!';
            ROLLBACK TRANSACTION; RETURN;
        END

        -- 3. Kiểm tra Sinh viên trùng ca
        DECLARE @TrungSV INT;
        SELECT @TrungSV = COUNT(DISTINCT dk_moi.MaSV)
        FROM DangKyHocPhan dk_moi
        WHERE dk_moi.MaLopHP = @MaLopHP
        AND EXISTS (
            SELECT 1 FROM DangKyHocPhan dk_cu
            INNER JOIN LichThi lt_cu ON dk_cu.MaLopHP = lt_cu.MaLopHP
            WHERE dk_cu.MaSV = dk_moi.MaSV
            AND lt_cu.NgayThi = @NgayThi AND lt_cu.CaThi = @CaThi
            AND lt_cu.MaLopHP != @MaLopHP
        );

        IF @TrungSV > 0
        BEGIN
            SET @Message = N'Lỗi: Phát hiện ' + CAST(@TrungSV AS NVARCHAR(10)) + N' sinh viên bị TRÙNG LỊCH THI với một môn khác. Vui lòng xếp ca/ngày khác!';
            ROLLBACK TRANSACTION; RETURN;
        END

        -- NẾU OK THÌ CẬP NHẬT
        UPDATE LichThi
        SET NgayThi = @NgayThi, CaThi = @CaThi, PhongThi = @PhongThi
        WHERE MaLopHP = @MaLopHP;
        
        COMMIT TRANSACTION;
        SET @Message = 'Success';
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SET @Message = ERROR_MESSAGE();
    END CATCH
END

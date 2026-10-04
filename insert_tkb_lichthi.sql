-- XÓA TKB VÀ LỊCH THI CŨ CỦA CÁC LỚP NÀY ĐỂ TRÁNH LỖI
DELETE FROM ThoiKhoaBieu WHERE MaLopHP IN (SELECT MaLopHP FROM LopHocPhan WHERE MaLopHP LIKE '%_01');
DELETE FROM LichThi WHERE MaLopHP IN (SELECT MaLopHP FROM LopHocPhan WHERE MaLopHP LIKE '%_01');

DECLARE @MaLopHP VARCHAR(20);
DECLARE @MaGV VARCHAR(20);
DECLARE @NgayHoc INT = 2; 
DECLARE @CaHoc INT = 1; 
DECLARE @PhongHoc VARCHAR(50) = 'A1-101';

DECLARE @NgayThi DATE = '2027-01-15';
DECLARE @CaThi INT = 1;

DECLARE cur CURSOR FOR 
SELECT MaLopHP, MaGV FROM LopHocPhan WHERE MaLopHP LIKE '%_01' ORDER BY MaLopHP;

OPEN cur;
FETCH NEXT FROM cur INTO @MaLopHP, @MaGV;

WHILE @@FETCH_STATUS = 0
BEGIN
    IF @CaHoc = 1 SET @PhongHoc = 'A1-101';
    IF @CaHoc = 2 SET @PhongHoc = 'A1-102';
    IF @CaHoc = 3 SET @PhongHoc = 'A2-205';
    IF @CaHoc = 4 SET @PhongHoc = 'B1-101';

    BEGIN TRY
        INSERT INTO ThoiKhoaBieu (MaLopHP, NgayHoc, CaHoc, PhongHoc) 
        VALUES (@MaLopHP, @NgayHoc, @CaHoc, @PhongHoc);
    END TRY
    BEGIN CATCH 
        PRINT ERROR_MESSAGE(); 
    END CATCH

    BEGIN TRY
        DECLARE @Msg NVARCHAR(255);
        EXEC sp_AddLichThi @MaLopHP, @NgayThi, @CaThi, @PhongHoc, @Msg OUTPUT;
    END TRY
    BEGIN CATCH 
        PRINT ERROR_MESSAGE(); 
    END CATCH

    SET @CaHoc = @CaHoc + 1;
    IF @CaHoc > 4 
    BEGIN
        SET @CaHoc = 1;
        SET @NgayHoc = @NgayHoc + 1;
        IF @NgayHoc > 7 SET @NgayHoc = 2;
    END

    SET @CaThi = @CaThi + 1;
    IF @CaThi > 4
    BEGIN
        SET @CaThi = 1;
        SET @NgayThi = DATEADD(day, 1, @NgayThi);
    END

    FETCH NEXT FROM cur INTO @MaLopHP, @MaGV;
END

CLOSE cur;
DEALLOCATE cur;

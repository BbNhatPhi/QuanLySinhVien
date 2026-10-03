ALTER PROCEDURE [dbo].[sp_GetThongTinCaNhan]
    @Username NVARCHAR(50)
AS
BEGIN
    SELECT 
        sv.MaSV, sv.HoTen, sv.GioiTinh, sv.NgaySinh, sv.Email, sv.SoDienThoai, sv.DiaChi,
        k.TenKhoa, n.TenNganh, sv.NganhID
    FROM SinhVien sv
    INNER JOIN Users u ON sv.UserID = u.UserID
    LEFT JOIN Khoa k ON sv.KhoaID = k.KhoaID
    LEFT JOIN Nganh n ON sv.NganhID = n.NganhID
    WHERE u.Username = @Username
END

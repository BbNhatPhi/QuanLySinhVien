import codecs
import re

with codecs.open('DAO/StudentDAO.cs', 'r', 'utf-8-sig') as f:
    text = f.read()

# Modify method signature
text = text.replace('public List<LopHocPhan> GetLopHocPhanMoDangKy()', 'public List<LopHocPhan> GetLopHocPhanMoDangKy(string username)')

# Modify SQL query
old_sql = r'''                    string sql = @"
                        SELECT lhp.MaLopHP, m.TenMon, m.SoTinChi, gv.HoTen AS TenGV, lhp.HocKy, lhp.NamHoc, 
                               lhp.SoLuongMax, COUNT\(dk.MaSV\) AS SiSoHienTai
                        FROM LopHocPhan lhp
                        INNER JOIN MonHoc m ON lhp.MaMon = m.MaMon
                        INNER JOIN GiangVien gv ON lhp.MaGV = gv.MaGV
                        LEFT JOIN DangKyHocPhan dk ON lhp.MaLopHP = dk.MaLopHP
                        GROUP BY lhp.MaLopHP, m.TenMon, m.SoTinChi, gv.HoTen, lhp.HocKy, lhp.NamHoc, lhp.SoLuongMax";
                    SqlCommand cmd = new SqlCommand\(sql, conn\);'''

new_sql = r'''                    string sql = @"
                        SELECT lhp.MaLopHP, m.TenMon, m.SoTinChi, gv.HoTen AS TenGV, lhp.HocKy, lhp.NamHoc, 
                               lhp.SoLuongMax, COUNT(dk.MaSV) AS SiSoHienTai
                        FROM LopHocPhan lhp
                        INNER JOIN MonHoc m ON lhp.MaMon = m.MaMon
                        INNER JOIN GiangVien gv ON lhp.MaGV = gv.MaGV
                        LEFT JOIN DangKyHocPhan dk ON lhp.MaLopHP = dk.MaLopHP
                        -- LỌC CHỈ NHỮNG MÔN THUỘC CHƯƠNG TRÌNH KHUNG CỦA SINH VIÊN
                        INNER JOIN ChuongTrinhKhung ctk ON ctk.MaMon = m.MaMon
                        INNER JOIN SinhVien sv ON sv.NganhID = ctk.NganhID
                        INNER JOIN Users u ON sv.UserID = u.UserID
                        WHERE u.Username = @Username
                        GROUP BY lhp.MaLopHP, m.TenMon, m.SoTinChi, gv.HoTen, lhp.HocKy, lhp.NamHoc, lhp.SoLuongMax";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Username", username);'''

text = re.sub(old_sql, new_sql, text)

with codecs.open('DAO/StudentDAO.cs', 'w', 'utf-8-sig') as f:
    f.write(text)

import codecs
import re

with codecs.open('DAO/StudentDAO.cs', 'r', 'utf-8-sig') as f:
    text = f.read()

# Replace the method properly
pattern = r'public List<LopHocPhan> GetLopHocPhanMoDangKy\(string username\)[\s\S]*?return list;\s*\}'
new_method = '''public List<LopHocPhan> GetLopHocPhanMoDangKy(string username)
        {
            List<LopHocPhan> list = new List<LopHocPhan>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
                    SELECT lhp.MaLopHP, m.TenMon, m.SoTinChi, gv.HoTen AS TenGV, lhp.HocKy, lhp.NamHoc, 
                           lhp.SoLuongMax, COUNT(dk.MaSV) AS SiSoHienTai
                    FROM LopHocPhan lhp
                    INNER JOIN MonHoc m ON lhp.MaMon = m.MaMon
                    INNER JOIN GiangVien gv ON lhp.MaGV = gv.MaGV
                    LEFT JOIN DangKyHocPhan dk ON lhp.MaLopHP = dk.MaLopHP
                    INNER JOIN ChuongTrinhKhung ctk ON ctk.MaMon = m.MaMon
                    INNER JOIN SinhVien sv ON sv.NganhID = ctk.NganhID
                    INNER JOIN Users u ON sv.UserID = u.UserID
                    WHERE u.Username = @Username AND lhp.TrangThai = N'Mở đăng ký'
                    GROUP BY lhp.MaLopHP, m.TenMon, m.SoTinChi, gv.HoTen, lhp.HocKy, lhp.NamHoc, lhp.SoLuongMax";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Username", username);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new LopHocPhan
                        {
                            MaLopHP = reader["MaLopHP"].ToString(),
                            TenMon = reader["TenMon"].ToString(),
                            SoTinChi = Convert.ToInt32(reader["SoTinChi"]),
                            TenGV = reader["TenGV"].ToString(),
                            HocKy = Convert.ToInt32(reader["HocKy"]),
                            NamHoc = reader["NamHoc"].ToString(),
                            SoLuongMax = Convert.ToInt32(reader["SoLuongMax"]),
                            TrangThai = reader["SiSoHienTai"].ToString()
                        });
                    }
                }
            }
            return list;
        }'''

text = re.sub(pattern, new_method, text)

with codecs.open('DAO/StudentDAO.cs', 'w', 'utf-8-sig') as f:
    f.write(text)

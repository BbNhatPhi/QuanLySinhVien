import codecs
import re

with codecs.open('DAO/StudentDAO.cs', 'r', 'utf-8-sig') as f:
    text = f.read()

new_method = r'''
        public List<ChuongTrinhKhungItem> GetChuongTrinhKhung(string username)
        {
            List<ChuongTrinhKhungItem> list = new List<ChuongTrinhKhungItem>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
                    SELECT 
                        c.MaMon, m.TenMon, m.SoTinChi, c.HocKyTieuChuan,
                        (SELECT TOP 1 
                            CASE 
                                WHEN d.DiemTong >= 4.0 THEN N'Qua môn'
                                WHEN d.DiemTong < 4.0 AND (d.DiemCC > 0 OR d.DiemGK > 0) THEN N'Học lại'
                                ELSE N'Đang học'
                            END
                         FROM Diem d 
                         INNER JOIN LopHocPhan lhp ON d.MaLopHP = lhp.MaLopHP
                         WHERE lhp.MaMon = c.MaMon AND d.MaSV = sv.MaSV
                         ORDER BY lhp.NamHoc DESC, lhp.HocKy DESC
                        ) AS TrangThaiMon
                    FROM ChuongTrinhKhung c
                    INNER JOIN MonHoc m ON c.MaMon = m.MaMon
                    INNER JOIN SinhVien sv ON sv.NganhID = c.NganhID
                    INNER JOIN Users u ON u.UserID = sv.UserID
                    WHERE u.Username = @Username
                    ORDER BY c.HocKyTieuChuan, m.TenMon";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Username", username);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ChuongTrinhKhungItem
                        {
                            MaMon = reader["MaMon"].ToString(),
                            TenMon = reader["TenMon"].ToString(),
                            SoTinChi = Convert.ToInt32(reader["SoTinChi"]),
                            HocKyTieuChuan = Convert.ToInt32(reader["HocKyTieuChuan"]),
                            TrangThai = reader["TrangThaiMon"] != DBNull.Value ? reader["TrangThaiMon"].ToString() : "Chưa học"
                        });
                    }
                }
            }
            return list;
        }
'''

# Insert before the last two closing braces
parts = text.rsplit('}', 2)
if len(parts) == 3:
    text = parts[0] + new_method + '\n    }\n}'
    with codecs.open('DAO/StudentDAO.cs', 'w', 'utf-8-sig') as f:
        f.write(text)

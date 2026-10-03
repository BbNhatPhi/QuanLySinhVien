import codecs

with codecs.open('DAO/TeacherDAO.cs', 'r', 'utf-8-sig') as f:
    text = f.read()

# Fix Dang h?c issue
text = text.replace(u"sv.TrangThaiHocTap = N'Dang h?c'", u"sv.TrangThaiHocTap = N'Đang học'")

# Add methods before last braces
new_methods = u'''
        // =====================================
        // TÍNH NĂNG ĐIỂM DANH, THÔNG BÁO VÀ TÀI LIỆU
        // =====================================
        public List<DiemDanh> GetDiemDanh(string maLopHP, DateTime ngayHoc)
        {
            List<DiemDanh> list = new List<DiemDanh>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
                    SELECT sv.MaSV, sv.HoTen, ISNULL(dd.TrangThai, N'Chưa điểm danh') AS TrangThai, dd.GhiChu
                    FROM SinhVien sv
                    INNER JOIN DangKyHocPhan dk ON sv.MaSV = dk.MaSV
                    LEFT JOIN DiemDanh dd ON sv.MaSV = dd.MaSV AND dd.MaLopHP = dk.MaLopHP AND CAST(dd.NgayHoc AS DATE) = CAST(@NgayHoc AS DATE)
                    WHERE dk.MaLopHP = @MaLopHP AND sv.TrangThaiHocTap = N'Đang học'
                    ORDER BY sv.MaSV";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaLopHP", maLopHP);
                cmd.Parameters.AddWithValue("@NgayHoc", ngayHoc.Date);
                
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new DiemDanh
                        {
                            MaSV = reader["MaSV"].ToString(),
                            HoTenSV = reader["HoTen"].ToString(),
                            TrangThai = reader["TrangThai"].ToString(),
                            GhiChu = reader["GhiChu"] != DBNull.Value ? reader["GhiChu"].ToString() : ""
                        });
                    }
                }
            }
            return list;
        }

        public void LuuDiemDanh(string maLopHP, string maSV, DateTime ngayHoc, string trangThai, string ghiChu)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
                    IF EXISTS (SELECT 1 FROM DiemDanh WHERE MaLopHP = @MaLopHP AND MaSV = @MaSV AND CAST(NgayHoc AS DATE) = CAST(@NgayHoc AS DATE))
                    BEGIN
                        UPDATE DiemDanh 
                        SET TrangThai = @TrangThai, GhiChu = @GhiChu
                        WHERE MaLopHP = @MaLopHP AND MaSV = @MaSV AND CAST(NgayHoc AS DATE) = CAST(@NgayHoc AS DATE)
                    END
                    ELSE
                    BEGIN
                        INSERT INTO DiemDanh(MaLopHP, MaSV, NgayHoc, TrangThai, GhiChu)
                        VALUES(@MaLopHP, @MaSV, @NgayHoc, @TrangThai, @GhiChu)
                    END";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaLopHP", maLopHP);
                cmd.Parameters.AddWithValue("@MaSV", maSV);
                cmd.Parameters.AddWithValue("@NgayHoc", ngayHoc.Date);
                cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                cmd.Parameters.AddWithValue("@GhiChu", (object)ghiChu ?? DBNull.Value);
                
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<ThongBaoLop> GetThongBaoLop(string maLopHP)
        {
            List<ThongBaoLop> list = new List<ThongBaoLop>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = "SELECT * FROM ThongBaoLop WHERE MaLopHP = @MaLopHP ORDER BY NgayDang DESC";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaLopHP", maLopHP);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ThongBaoLop
                        {
                            ThongBaoID = Convert.ToInt32(reader["ThongBaoID"]),
                            MaLopHP = reader["MaLopHP"].ToString(),
                            TieuDe = reader["TieuDe"].ToString(),
                            NoiDung = reader["NoiDung"].ToString(),
                            NgayDang = Convert.ToDateTime(reader["NgayDang"])
                        });
                    }
                }
            }
            return list;
        }

        public void AddThongBaoLop(ThongBaoLop tb)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = "INSERT INTO ThongBaoLop (MaLopHP, TieuDe, NoiDung) VALUES (@MaLopHP, @TieuDe, @NoiDung)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaLopHP", tb.MaLopHP);
                cmd.Parameters.AddWithValue("@TieuDe", tb.TieuDe);
                cmd.Parameters.AddWithValue("@NoiDung", tb.NoiDung);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<TaiLieuLop> GetTaiLieuLop(string maLopHP)
        {
            List<TaiLieuLop> list = new List<TaiLieuLop>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = "SELECT * FROM TaiLieuLop WHERE MaLopHP = @MaLopHP ORDER BY NgayTaiLen DESC";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaLopHP", maLopHP);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new TaiLieuLop
                        {
                            TaiLieuID = Convert.ToInt32(reader["TaiLieuID"]),
                            MaLopHP = reader["MaLopHP"].ToString(),
                            TenTaiLieu = reader["TenTaiLieu"].ToString(),
                            DuongDan = reader["DuongDan"].ToString(),
                            NgayTaiLen = Convert.ToDateTime(reader["NgayTaiLen"])
                        });
                    }
                }
            }
            return list;
        }

        public void AddTaiLieuLop(TaiLieuLop tl)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = "INSERT INTO TaiLieuLop (MaLopHP, TenTaiLieu, DuongDan) VALUES (@MaLopHP, @TenTaiLieu, @DuongDan)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaLopHP", tl.MaLopHP);
                cmd.Parameters.AddWithValue("@TenTaiLieu", tl.TenTaiLieu);
                cmd.Parameters.AddWithValue("@DuongDan", tl.DuongDan);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
'''

if 'GetDiemDanh' not in text:
    # Insert before the last two closing braces
    parts = text.rsplit('}', 2)
    if len(parts) == 3:
        text = parts[0] + new_methods + '\n    }\n}'
        with codecs.open('DAO/TeacherDAO.cs', 'w', 'utf-8-sig') as f:
            f.write(text)
        print("Updated TeacherDAO.cs")
else:
    print("Already in TeacherDAO.cs")

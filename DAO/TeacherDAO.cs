using StudentManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
namespace StudentManagementSystem.DAO
{
    public class TeacherDAO
    {
        private string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // 1. Láº¥y lá»‹ch dáº¡y cá»§a Giáº£ng viÃªn (ÄÃ£ káº¿t ná»‘i vá»›i báº£ng ThoiKhoaBieu thá»±c táº¿)
        public List<LichDay> GetLichDay(string username)
        {
            List<LichDay> list = new List<LichDay>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
                    SELECT lhp.MaLopHP, m.TenMon, 
                           ISNULL(tkb.PhongHoc, N'ChÆ°a xáº¿p phÃ²ng') AS PhongHoc, 
                           ISNULL(tkb.NgayHoc, 0) AS NgayHoc, 
                           ISNULL(tkb.CaHoc, 0) AS CaHoc
                    FROM Users u
                    INNER JOIN GiangVien gv ON u.UserID = gv.UserID
                    INNER JOIN LopHocPhan lhp ON gv.MaGV = lhp.MaGV
                    LEFT JOIN ThoiKhoaBieu tkb ON lhp.MaLopHP = tkb.MaLopHP
                    INNER JOIN MonHoc m ON lhp.MaMon = m.MaMon
                    WHERE u.Username = @Username
                    ORDER BY tkb.NgayHoc ASC, tkb.CaHoc ASC";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Username", username);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new LichDay
                        {
                            MaLopHP = reader["MaLopHP"].ToString(),
                            TenMon = reader["TenMon"].ToString(),
                            PhongHoc = reader["PhongHoc"].ToString(),
                            NgayHoc = Convert.ToInt32(reader["NgayHoc"]),
                            CaHoc = Convert.ToInt32(reader["CaHoc"])
                        });
                    }
                }
            }
            return list;
        }

        // 2. Láº¥y danh sÃ¡ch Ä‘iá»ƒm
        public List<SinhVienDiem> GetDanhSachDiem(string maLopHP)
        {
            List<SinhVienDiem> list = new List<SinhVienDiem>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
                    SELECT sv.MaSV, sv.HoTen, d.DiemCC, d.DiemGK, d.DiemCK, d.DiemTong
                    FROM Diem d
                    INNER JOIN SinhVien sv ON d.MaSV = sv.MaSV
                    WHERE d.MaLopHP = @MaLopHP AND sv.TrangThaiHocTap = N'Đang học'
                    ORDER BY sv.MaSV";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaLopHP", maLopHP);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new SinhVienDiem
                        {
                            MaSV = reader["MaSV"].ToString(),
                            HoTen = reader["HoTen"].ToString(),
                            DiemCC = reader["DiemCC"] != DBNull.Value ? Convert.ToDouble(reader["DiemCC"]) : 0,
                            DiemGK = reader["DiemGK"] != DBNull.Value ? Convert.ToDouble(reader["DiemGK"]) : 0,
                            DiemCK = reader["DiemCK"] != DBNull.Value ? Convert.ToDouble(reader["DiemCK"]) : 0,
                            DiemTong = reader["DiemTong"] != DBNull.Value ? Convert.ToDouble(reader["DiemTong"]) : 0
                        });
                    }
                }
            }
            return list;
        }

        // 3. Cáº­p nháº­t Ä‘iá»ƒm cho Sinh viÃªn
        public void UpdateDiem(string maSV, string maLopHP, double diemCC, double diemGK, double diemCK)
        {
            double diemTong = (diemCC * 0.1) + (diemGK * 0.3) + (diemCK * 0.6);
            diemTong = Math.Round(diemTong, 1);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
                    IF EXISTS (SELECT 1 FROM Diem WHERE MaSV = @MaSV AND MaLopHP = @MaLopHP)
                    BEGIN
                        UPDATE Diem 
                        SET DiemCC = @CC, DiemGK = @GK, DiemCK = @CK, DiemTong = @Tong
                        WHERE MaSV = @MaSV AND MaLopHP = @MaLopHP;
                    END
                    ELSE
                    BEGIN
                        INSERT INTO Diem (MaSV, MaLopHP, DiemCC, DiemGK, DiemCK, DiemTong)
                        VALUES (@MaSV, @MaLopHP, @CC, @GK, @CK, @Tong);
                    END";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@CC", diemCC);
                cmd.Parameters.AddWithValue("@GK", diemGK);
                cmd.Parameters.AddWithValue("@CK", diemCK);
                cmd.Parameters.AddWithValue("@Tong", diemTong);
                cmd.Parameters.AddWithValue("@MaSV", maSV);
                cmd.Parameters.AddWithValue("@MaLopHP", maLopHP);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // 4. Láº¥y danh sÃ¡ch chi tiáº¿t sinh viÃªn (Phá»¥c vá»¥ Äiá»ƒm danh)
        public List<SinhVienLop> GetDanhSachSinhVien(string maLopHP)
        {
            List<SinhVienLop> list = new List<SinhVienLop>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
                    SELECT sv.MaSV, sv.HoTen, sv.GioiTinh, sv.NgaySinh, sv.Email, d.DiemCC
                    FROM Diem d
                    INNER JOIN SinhVien sv ON d.MaSV = sv.MaSV
                    WHERE d.MaLopHP = @MaLopHP AND sv.TrangThaiHocTap = N'Đang học'
                    ORDER BY sv.MaSV";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaLopHP", maLopHP);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new SinhVienLop
                        {
                            MaSV = reader["MaSV"].ToString(),
                            HoTen = reader["HoTen"].ToString(),
                            GioiTinh = reader["GioiTinh"].ToString(),
                            NgaySinh = reader["NgaySinh"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["NgaySinh"]) : null,
                            Email = reader["Email"].ToString(),
                            DiemCC = reader["DiemCC"] != DBNull.Value ? Convert.ToDouble(reader["DiemCC"]) : 0
                        });
                    }
                }
            }
            return list;
        }

        // 5. HÃ m trá»« Ä‘iá»ƒm chuyÃªn cáº§n khi váº¯ng máº·t (tá»± Ä‘á»™ng cáº­p nháº­t láº¡i Äiá»ƒm Tá»•ng káº¿t)
        public void TruDiemChuyenCan(string maSV, string maLopHP)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"UPDATE Diem 
                               SET DiemCC = CASE WHEN DiemCC >= 1 THEN DiemCC - 1 ELSE 0 END,
                                   DiemTong = ROUND(((CASE WHEN DiemCC >= 1 THEN DiemCC - 1 ELSE 0 END) * 0.1) + (ISNULL(DiemGK, 0) * 0.3) + (ISNULL(DiemCK, 0) * 0.6), 1)
                               WHERE MaSV = @MaSV AND MaLopHP = @MaLopHP";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaSV", maSV);
                cmd.Parameters.AddWithValue("@MaLopHP", maLopHP);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public void TruDiemVangHoc(string maSV, string hocKy, string trangThaiVang)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_TruDiemRenLuyen", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@MaSV", maSV);
                cmd.Parameters.AddWithValue("@HocKy", hocKy);
                cmd.Parameters.AddWithValue("@TrangThaiVang", trangThaiVang); // Truyá»n 'CÃ³ phÃ©p' hoáº·c 'KhÃ´ng phÃ©p'

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public string GetHocKyByMaLopHP(string maLopHP)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = "SELECT HocKy, NamHoc FROM LopHocPhan WHERE MaLopHP = @MaLopHP";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaLopHP", maLopHP);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return string.Format("HK{0} ({1})", reader["HocKy"], reader["NamHoc"]);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Láº¥y danh sÃ¡ch email sinh viÃªn Ä‘Äƒng kÃ½ lá»›p há»c pháº§n nÃ y (Ä‘á»ƒ gá»­i email thÃ´ng bÃ¡o Ä‘iá»ƒm)
        /// Returns list of KeyValuePair(HoTen, Email)
        /// </summary>
        public List<System.Collections.Generic.KeyValuePair<string, string>> GetEmailsSinhVienByLop(string maLopHP)
        {
            var result = new List<System.Collections.Generic.KeyValuePair<string, string>>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
                    SELECT sv.HoTen, sv.Email
                    FROM DangKyHocPhan dkhp
                    INNER JOIN SinhVien sv ON dkhp.MaSV = sv.MaSV
                    WHERE dkhp.MaLopHP = @MaLopHP AND sv.Email IS NOT NULL AND sv.Email <> ''";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaLopHP", maLopHP);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new System.Collections.Generic.KeyValuePair<string, string>(
                            reader["HoTen"].ToString(), reader["Email"].ToString()));
                    }
                }
            }
            return result;
        }
    
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

    }
}
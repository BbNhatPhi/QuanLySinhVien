using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using StudentManagementSystem.Models;
namespace StudentManagementSystem.DAO
{
    public class StudentDAO
    {
        private string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // ==========================================
        // 1. TÍNH NĂNG ĐĂNG KÝ HỌC PHẦN 
        // ==========================================

        public List<LopHocPhan> GetLopHocPhanMoDangKy()
        {
            List<LopHocPhan> list = new List<LopHocPhan>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
                    SELECT lhp.MaLopHP, m.TenMon, gv.HoTen AS TenGV, lhp.HocKy, lhp.NamHoc, 
                           lhp.SoLuongMax, COUNT(dk.MaSV) AS SiSoHienTai
                    FROM LopHocPhan lhp
                    INNER JOIN MonHoc m ON lhp.MaMon = m.MaMon
                    INNER JOIN GiangVien gv ON lhp.MaGV = gv.MaGV
                    LEFT JOIN DangKyHocPhan dk ON lhp.MaLopHP = dk.MaLopHP
                    WHERE lhp.TrangThai = N'Mở đăng ký'
                    GROUP BY lhp.MaLopHP, m.TenMon, gv.HoTen, lhp.HocKy, lhp.NamHoc, lhp.SoLuongMax";

                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new LopHocPhan
                        {
                            MaLopHP = reader["MaLopHP"].ToString(),
                            TenMon = reader["TenMon"].ToString(),
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
        }

        public string DangKyLop(string username, string maLopHP)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_SinhVienDangKyHoc", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@MaLopHP", maLopHP);

                // Khai báo biến Output để hứng kết quả từ SQL (Success hoặc thông báo lỗi)
                SqlParameter msgParam = new SqlParameter("@Message", SqlDbType.NVarChar, 255);
                msgParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(msgParam);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    return msgParam.Value.ToString();
                }
                catch (Exception ex)
                {
                    return "Lỗi hệ thống: " + ex.Message;
                }
            }
        }

        // ==========================================
        // 2. XEM THỜI KHÓA BIỂU CỦA SINH VIÊN
        // ==========================================
        public List<ThoiKhoaBieuSV> GetTKB(string username)
        {
            List<ThoiKhoaBieuSV> list = new List<ThoiKhoaBieuSV>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
                    SELECT mh.TenMon, lhp.MaLopHP, tkb.NgayHoc, tkb.CaHoc, tkb.PhongHoc, gv.HoTen AS TenGiangVien
                    FROM ThoiKhoaBieu tkb
                    INNER JOIN LopHocPhan lhp ON tkb.MaLopHP = lhp.MaLopHP
                    INNER JOIN MonHoc mh ON lhp.MaMon = mh.MaMon
                    INNER JOIN GiangVien gv ON lhp.MaGV = gv.MaGV
                    INNER JOIN DangKyHocPhan dk ON lhp.MaLopHP = dk.MaLopHP
                    INNER JOIN SinhVien sv ON dk.MaSV = sv.MaSV
                    INNER JOIN Users u ON sv.UserID = u.UserID
                    WHERE u.Username = @Username
                    ORDER BY tkb.NgayHoc, tkb.CaHoc";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Username", username);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ThoiKhoaBieuSV
                        {
                            TenMon = reader["TenMon"].ToString(),
                            MaLopHP = reader["MaLopHP"].ToString(), // Đã sửa tên biến hứng dữ liệu
                            NgayHoc = Convert.ToInt32(reader["NgayHoc"]),
                            CaHoc = Convert.ToInt32(reader["CaHoc"]),
                            PhongHoc = reader["PhongHoc"].ToString(),
                            TenGiangVien = reader["TenGiangVien"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        // ==========================================
        // 3. XEM BẢNG ĐIỂM CỦA SINH VIÊN
        // ==========================================
        public List<KetQuaHocTap> GetBangDiem(string username)
        {
            List<KetQuaHocTap> list = new List<KetQuaHocTap>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
                    SELECT mh.TenMon, mh.SoTinChi, lhp.HocKy, lhp.NamHoc, 
                           d.DiemCC, d.DiemGK, d.DiemCK, d.DiemTong
                    FROM Diem d
                    INNER JOIN LopHocPhan lhp ON d.MaLopHP = lhp.MaLopHP
                    INNER JOIN MonHoc mh ON lhp.MaMon = mh.MaMon
                    INNER JOIN SinhVien sv ON d.MaSV = sv.MaSV
                    INNER JOIN Users u ON sv.UserID = u.UserID
                    WHERE u.Username = @Username
                    ORDER BY lhp.NamHoc DESC, lhp.HocKy DESC";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Username", username);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new KetQuaHocTap
                        {
                            TenMon = reader["TenMon"].ToString(),
                            SoTinChi = Convert.ToInt32(reader["SoTinChi"]),
                            HocKy = Convert.ToInt32(reader["HocKy"]),
                            NamHoc = reader["NamHoc"].ToString(),
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
        // ==========================================
        // 4. XEM HỌC PHÍ CỦA SINH VIÊN
        // ==========================================
        public List<HocPhi> GetHocPhi(string username)
        {
            List<HocPhi> list = new List<HocPhi>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_GetHocPhiSinhVien", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username", username);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new HocPhi
                        {
                            MaLopHP = reader["MaLopHP"].ToString(),
                            TenMon = reader["TenMon"].ToString(),
                            SoTinChi = Convert.ToInt32(reader["SoTinChi"]),
                            DonGia = Convert.ToDouble(reader["DonGia"])
                        });
                    }
                }
            }
            return list;
        }
        // ==========================================
        // 5. XEM THÔNG BÁO TỪ NHÀ TRƯỜNG
        // ==========================================
        public List<ThongBao> GetThongBao()
        {
            List<ThongBao> list = new List<ThongBao>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_GetThongBaoSinhVien", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ThongBao
                        {
                            MaTB = Convert.ToInt32(reader["MaTB"]),
                            TieuDe = reader["TieuDe"].ToString(),
                            NoiDung = reader["NoiDung"].ToString(),
                            NgayDang = Convert.ToDateTime(reader["NgayDang"]),
                            NguoiDang = reader["NguoiDang"].ToString()
                        });
                    }
                }
            }
            return list;
        }
        // ==========================================
        // 6. CẬP NHẬT THÔNG TIN CÁ NHÂN
        // ==========================================

        public SinhVien GetThongTinCaNhan(string username)
        {
            SinhVien sv = null;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_GetThongTinCaNhan", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username", username);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        sv = new SinhVien
                        {
                            MaSV = reader["MaSV"].ToString(),
                            HoTen = reader["HoTen"].ToString(),
                            GioiTinh = reader["GioiTinh"].ToString(),
                            NgaySinh = reader["NgaySinh"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["NgaySinh"]) : null,
                            Email = reader["Email"].ToString(),
                            // Dùng Check DBNull để tránh lỗi nếu dữ liệu rỗng
                            SoDienThoai = reader["SoDienThoai"] != DBNull.Value ? reader["SoDienThoai"].ToString() : "",
                            DiaChi = reader["DiaChi"] != DBNull.Value ? reader["DiaChi"].ToString() : ""
                        };
                    }
                }
            }
            return sv;
        }

        public string UpdateThongTinCaNhan(SinhVien sv)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_UpdateThongTinCaNhan", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@MaSV", sv.MaSV);
                cmd.Parameters.AddWithValue("@Email", (object)sv.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SoDienThoai", (object)sv.SoDienThoai ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DiaChi", (object)sv.DiaChi ?? DBNull.Value);

                SqlParameter msgParam = new SqlParameter("@Message", SqlDbType.NVarChar, 255);
                msgParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(msgParam);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    return msgParam.Value.ToString();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
        public List<LichThi> GetLichThiSinhVien(string username)
        {
            List<LichThi> list = new List<LichThi>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_GetLichThiSinhVien", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username", username);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new LichThi
                        {
                            MaLopHP = reader["MaLopHP"].ToString(),
                            TenMon = reader["TenMon"].ToString(),
                            NgayThi = Convert.ToDateTime(reader["NgayThi"]),
                            CaThi = Convert.ToInt32(reader["CaThi"]),
                            PhongThi = reader["PhongThi"].ToString()
                        });
                    }
                }
            }
            return list;
        }
        public void ThanhToanHocPhi(string username, string maLopHP)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_ThanhToanHocPhi", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@MaLopHP", maLopHP);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public List<HocPhi> GetLichSuThanhToan(string username)
        {
            List<HocPhi> list = new List<HocPhi>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_GetLichSuThanhToanSinhVien", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username", username);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new HocPhi
                        {
                            MaLopHP = reader["MaLopHP"].ToString(),
                            TenMon = reader["TenMon"].ToString(),
                            SoTinChi = Convert.ToInt32(reader["SoTinChi"]),
                            DonGia = Convert.ToDouble(reader["DonGia"]),                         
                        });
                    }
                }
            }
            return list;
        }
        public TienDoHocTap GetTienDoHocTap(string username)
        {
            TienDoHocTap tienDo = new TienDoHocTap();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_GetTienDoHocTap", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username", username);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        tienDo.TinChiTichLuy = Convert.ToInt32(reader["TinChiTichLuy"]);
                        tienDo.TongTinChiDaHoc = Convert.ToInt32(reader["TongTinChiDaHoc"]);
                        tienDo.GPA_He10 = Convert.ToDouble(reader["GPA_He10"]);
                        tienDo.GPA_He4 = Convert.ToDouble(reader["GPA_He4"]);
                        tienDo.SoMonNo = Convert.ToInt32(reader["SoMonNo"]);
                    }
                }
            }
            return tienDo;
        }
        // Lấy lịch sử yêu cầu
        public List<YeuCauHanhChinh> GetLichSuYeuCau(string username)
        {
            List<YeuCauHanhChinh> list = new List<YeuCauHanhChinh>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_GetLichSuYeuCau", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username", username);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new YeuCauHanhChinh
                        {
                            MaYeuCau = Convert.ToInt32(reader["MaYeuCau"]),
                            LoaiDichVu = reader["LoaiDichVu"].ToString(),
                            LyDo = reader["LyDo"].ToString(),
                            NgayGui = Convert.ToDateTime(reader["NgayGui"]),
                            TrangThai = reader["TrangThai"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        // Gửi yêu cầu mới
        public string GuiYeuCauHanhChinh(string username, string loaiDichVu, string lyDo)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_GuiYeuCauHanhChinh", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@LoaiDichVu", loaiDichVu);
                cmd.Parameters.AddWithValue("@LyDo", lyDo);

                SqlParameter msgParam = new SqlParameter("@Message", SqlDbType.NVarChar, 255);
                msgParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(msgParam);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    return msgParam.Value.ToString();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
        // 1. Lấy danh sách các môn cần đánh giá
        public List<DanhGiaItem> GetDanhSachCanDanhGia(string username)
        {
            List<DanhGiaItem> list = new List<DanhGiaItem>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_GetDanhSachCanDanhGia", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username", username);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new DanhGiaItem
                        {
                            MaLopHP = reader["MaLopHP"].ToString(),
                            TenMon = reader["TenMon"].ToString(),
                            TenGV = reader["TenGV"].ToString(),
                            DaDanhGia = Convert.ToBoolean(reader["DaDanhGia"]),
                            DiemDanhGia = Convert.ToInt32(reader["DiemDanhGia"]),
                            NhanXet = reader["NhanXet"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        // 2. Lưu kết quả đánh giá
        public string LuuDanhGia(string username, string maLopHP, int diemDanhGia, string nhanXet)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_LuuDanhGiaGiangVien", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@MaLopHP", maLopHP);
                cmd.Parameters.AddWithValue("@DiemDanhGia", diemDanhGia);
                cmd.Parameters.AddWithValue("@NhanXet", nhanXet ?? "");

                SqlParameter msgParam = new SqlParameter("@Message", SqlDbType.NVarChar, 255);
                msgParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(msgParam);

                conn.Open();
                cmd.ExecuteNonQuery();
                return msgParam.Value.ToString();
            }
        }

        // 3. Kiểm tra số lượng môn chưa đánh giá để khóa/mở bảng điểm
        public int DemSoMonChuaDanhGia(string username)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_KiemTraChuaDanhGia", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username", username);

                SqlParameter countParam = new SqlParameter("@ChuaDanhGiaCount", SqlDbType.Int);
                countParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(countParam);

                conn.Open();
                cmd.ExecuteNonQuery();
                return countParam.Value != DBNull.Value ? Convert.ToInt32(countParam.Value) : 0;
            }
        }
    }
}
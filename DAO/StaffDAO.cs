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
    public class StaffDAO
    {
        private string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // ==========================================
        // QUẢN LÝ SINH VIÊN
        // ==========================================

        public List<SinhVien> GetAllSinhVien()
        {
            List<SinhVien> list = new List<SinhVien>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                // Dùng SQL thuần kết nối bảng SinhVien với bảng Khoa để lấy Tên Khoa (chữ)
                // ĐÃ SỬA: Dùng s.NganhID AS KhoaID để C# đọc được, và JOIN qua s.NganhID
                string sql = @"
                    SELECT s.MaSV, s.HoTen, s.NgaySinh, s.GioiTinh, s.DiaChi, s.Email, 
                           s.SoDienThoai, s.TrangThaiHocTap, s.NganhID AS KhoaID, k.TenKhoa 
                    FROM SinhVien s
                    LEFT JOIN Khoa k ON s.NganhID = k.KhoaID";

                SqlCommand cmd = new SqlCommand(sql, conn);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new SinhVien
                        {
                            MaSV = reader["MaSV"].ToString(),
                            HoTen = reader["HoTen"].ToString(),
                            NgaySinh = reader["NgaySinh"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["NgaySinh"]) : null,
                            GioiTinh = reader["GioiTinh"].ToString(),
                            DiaChi = reader["DiaChi"].ToString(),
                            Email = reader["Email"].ToString(),
                            SoDienThoai = reader["SoDienThoai"].ToString(),
                            TrangThaiHocTap = reader["TrangThaiHocTap"].ToString(),

                            // Ánh xạ chính xác cột Khoa từ SQL
                            KhoaID = reader["KhoaID"] != DBNull.Value ? Convert.ToInt32(reader["KhoaID"]) : 0,
                            TenKhoa = reader["TenKhoa"] != DBNull.Value ? reader["TenKhoa"].ToString() : ""
                        });
                    }
                }
            }
            return list;
        }

        public void UpdateTrangThaiHocTap(string maSV, string trangThaiMoi)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_UpdateTrangThaiHocTap", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@MaSV", maSV);
                cmd.Parameters.AddWithValue("@TrangThai", trangThaiMoi);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public string AddSinhVien(SinhVien sv)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_AddSinhVien", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@MaSV", sv.MaSV);
                cmd.Parameters.AddWithValue("@HoTen", (object)sv.HoTen ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@NgaySinh", (object)sv.NgaySinh ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@GioiTinh", (object)sv.GioiTinh ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", (object)sv.Email ?? DBNull.Value);

                // ĐÃ SỬA: Gửi tham số xuống SQL dưới tên @NganhID nhưng lấy dữ liệu từ sv.KhoaID
                cmd.Parameters.AddWithValue("@NganhID", sv.KhoaID);

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

        public bool DeleteSinhVien(string maSV)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_DeleteSinhVien", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MaSV", maSV);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch
                {
                    return false;
                }
            }
        }

        // ==========================================
        // QUẢN LÝ MÔN HỌC
        // ==========================================

        public List<MonHoc> GetAllMonHoc()
        {
            List<MonHoc> list = new List<MonHoc>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_GetAllMonHoc", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new MonHoc
                        {
                            MaMon = reader["MaMon"].ToString(),
                            TenMon = reader["TenMon"].ToString(),
                            SoTinChi = Convert.ToInt32(reader["SoTinChi"])
                        });
                    }
                }
            }
            return list;
        }

        public bool AddMonHoc(string maMon, string tenMon, int soTinChi)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_AddMonHoc", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@MaMon", maMon);
                    cmd.Parameters.AddWithValue("@TenMon", tenMon);
                    cmd.Parameters.AddWithValue("@SoTinChi", soTinChi);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool UpdateMonHoc(string maMon, string tenMon, int soTinChi)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_UpdateMonHoc", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@MaMon", maMon);
                    cmd.Parameters.AddWithValue("@TenMon", tenMon);
                    cmd.Parameters.AddWithValue("@SoTinChi", soTinChi);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch
                {
                    return false;
                }
            }
        }

        // ==========================================
        // QUẢN LÝ LỚP HỌC PHẦN (MỞ LỚP)
        // ==========================================

        public List<LopHocPhan> GetAllLopHocPhan()
        {
            List<LopHocPhan> list = new List<LopHocPhan>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_GetAllLopHocPhan", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new LopHocPhan
                        {
                            MaLopHP = reader["MaLopHP"].ToString(),
                            MaMon = reader["MaMon"].ToString(),
                            TenMon = reader["TenMon"].ToString(),
                            MaGV = reader["MaGV"].ToString(),
                            TenGV = reader["TenGV"].ToString(),
                            HocKy = Convert.ToInt32(reader["HocKy"]),
                            NamHoc = reader["NamHoc"].ToString(),
                            SoLuongMax = Convert.ToInt32(reader["SoLuongMax"]),
                            TrangThai = reader["TrangThai"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public string AddLopHocPhan(LopHocPhan lhp)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_AddLopHocPhan", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@MaLopHP", lhp.MaLopHP);
                cmd.Parameters.AddWithValue("@MaMon", lhp.MaMon);
                cmd.Parameters.AddWithValue("@MaGV", lhp.MaGV);
                cmd.Parameters.AddWithValue("@HocKy", lhp.HocKy);
                cmd.Parameters.AddWithValue("@NamHoc", lhp.NamHoc);
                cmd.Parameters.AddWithValue("@SoLuongMax", lhp.SoLuongMax);

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

        public List<GiangVien> GetAllGiangVien()
        {
            List<GiangVien> list = new List<GiangVien>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_GetAllGiangVien", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new GiangVien
                        {
                            MaGV = reader["MaGV"].ToString(),
                            HoTen = reader["HoTen"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        // ==========================================
        // QUẢN LÝ THỜI KHÓA BIỂU
        // ==========================================

        public List<ThoiKhoaBieu> GetAllTKB()
        {
            List<ThoiKhoaBieu> list = new List<ThoiKhoaBieu>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_GetAllTKB", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ThoiKhoaBieu
                        {
                            MaTKB = Convert.ToInt32(reader["MaTKB"]),
                            MaLopHP = reader["MaLopHP"].ToString(),
                            TenMon = reader["TenMon"].ToString(),
                            TenGV = reader["TenGV"].ToString(),
                            NgayHoc = Convert.ToInt32(reader["NgayHoc"]),
                            CaHoc = Convert.ToInt32(reader["CaHoc"]),
                            PhongHoc = reader["PhongHoc"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public string AddTKB(ThoiKhoaBieu tkb)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_AddTKB", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@MaLopHP", tkb.MaLopHP);
                cmd.Parameters.AddWithValue("@NgayHoc", tkb.NgayHoc);
                cmd.Parameters.AddWithValue("@CaHoc", tkb.CaHoc);
                cmd.Parameters.AddWithValue("@PhongHoc", tkb.PhongHoc);

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

        // ==========================================
        // QUẢN LÝ GIẢNG VIÊN VÀ KHOA
        // ==========================================

        public List<Khoa> GetAllKhoa()
        {
            List<Khoa> list = new List<Khoa>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_GetAllKhoa", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Khoa
                        {
                            KhoaID = Convert.ToInt32(reader["KhoaID"]),
                            TenKhoa = reader["TenKhoa"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public List<GiangVien> GetDanhSachGiangVien()
        {
            List<GiangVien> list = new List<GiangVien>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_GetDanhSachGiangVien", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new GiangVien
                        {
                            MaGV = reader["MaGV"].ToString(),
                            HoTen = reader["HoTen"].ToString(),
                            Email = reader["Email"].ToString(),
                            SoDienThoai = reader["SoDienThoai"].ToString(),
                            KhoaID = reader["KhoaID"] != DBNull.Value ? Convert.ToInt32(reader["KhoaID"]) : 0,
                            TenKhoa = reader["TenKhoa"].ToString(),
                            MaMon = reader["MaMon"].ToString(),
                            TenMon = reader["TenMon"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public string AddGiangVien(GiangVien gv)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_AddGiangVien", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@MaGV", gv.MaGV);
                cmd.Parameters.AddWithValue("@HoTen", gv.HoTen);
                cmd.Parameters.AddWithValue("@KhoaID", gv.KhoaID);
                cmd.Parameters.AddWithValue("@Email", (object)gv.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SoDienThoai", (object)gv.SoDienThoai ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@MaMon", DBNull.Value);

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

        public bool DeleteGiangVien(string maGV)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_DeleteGiangVien", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MaGV", maGV);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch
                {
                    return false;
                }
            }
        }

        // ==========================================
        // QUẢN LÝ LỊCH THI
        // ==========================================

        public List<LichThi> GetAllLichThi()
        {
            List<LichThi> list = new List<LichThi>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_GetAllLichThi", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new LichThi
                        {
                            MaLichThi = Convert.ToInt32(reader["MaLichThi"]),
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

        public string AddLichThi(string maLopHP, DateTime ngayThi, int caThi, string phongThi)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_AddLichThi", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MaLopHP", maLopHP);
                cmd.Parameters.AddWithValue("@NgayThi", ngayThi);
                cmd.Parameters.AddWithValue("@CaThi", caThi);
                cmd.Parameters.AddWithValue("@PhongThi", phongThi);

                SqlParameter msgParam = new SqlParameter("@Message", SqlDbType.NVarChar, 255);
                msgParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(msgParam);

                conn.Open();
                cmd.ExecuteNonQuery();
                return msgParam.Value.ToString();
            }
        }

        public string UpdateLichThi(string maLopHP, DateTime ngayThi, int caThi, string phongThi)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_UpdateLichThi", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MaLopHP", maLopHP);
                cmd.Parameters.AddWithValue("@NgayThi", ngayThi);
                cmd.Parameters.AddWithValue("@CaThi", caThi);
                cmd.Parameters.AddWithValue("@PhongThi", phongThi);

                SqlParameter msgParam = new SqlParameter("@Message", SqlDbType.NVarChar, 255);
                msgParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(msgParam);

                conn.Open();
                cmd.ExecuteNonQuery();
                return msgParam.Value.ToString();
            }
        }

        public bool DeleteLichThi(string maLopHP)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_DeleteLichThi", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MaLopHP", maLopHP);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch
                {
                    return false;
                }
            }
        }

        public List<PhongHoc> GetPhongTrongLichThi(DateTime ngayThi, int caThi)
        {
            List<PhongHoc> list = new List<PhongHoc>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_GetPhongTrongLichThi", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@NgayThi", ngayThi);
                cmd.Parameters.AddWithValue("@CaThi", caThi);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new PhongHoc
                        {
                            MaPhong = reader["MaPhong"].ToString(),
                            SucChua = Convert.ToInt32(reader["SucChua"]),
                            ToaNha = reader["ToaNha"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        // ==========================================
        // QUẢN LÝ THÔNG BÁO
        // ==========================================

        public List<ThongBao> GetAllThongBao()
        {
            List<ThongBao> list = new List<ThongBao>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = "SELECT MaTB, TieuDe, NoiDung, NgayDang, NguoiDang, DoiTuong FROM ThongBao ORDER BY NgayDang DESC";
                SqlCommand cmd = new SqlCommand(query, conn);

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
                            NguoiDang = reader["NguoiDang"].ToString(),
                            DoiTuong = reader["DoiTuong"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public bool AddThongBao(string tieuDe, string noiDung, string username, string doiTuong)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_AddThongBao", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@TieuDe", tieuDe);
                cmd.Parameters.AddWithValue("@NoiDung", noiDung);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@DoiTuong", doiTuong);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ==========================================
        // QUẢN LÝ ĐIỂM RÈN LUYỆN
        // ==========================================

        public List<DiemRenLuyen> GetAllDiemRenLuyen()
        {
            List<DiemRenLuyen> list = new List<DiemRenLuyen>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_GetAllDiemRenLuyen", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new DiemRenLuyen
                        {
                            ID = Convert.ToInt32(reader["ID"]),
                            MaSV = reader["MaSV"].ToString(),
                            HocKy = reader["HocKy"].ToString(),
                            Diem = Convert.ToInt32(reader["Diem"]),
                            XepLoai = reader["XepLoai"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public bool AddDiemRenLuyen(string maSV, string hocKy, int diem, string xepLoai)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_AddDiemRenLuyen", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MaSV", maSV);
                    cmd.Parameters.AddWithValue("@HocKy", hocKy);
                    cmd.Parameters.AddWithValue("@Diem", diem);
                    cmd.Parameters.AddWithValue("@XepLoai", xepLoai);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch { return false; }
            }
        }

        public bool UpdateDiemRenLuyen(int id, int diem, string xepLoai)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_UpdateDiemRenLuyen", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.Parameters.AddWithValue("@Diem", diem);
                    cmd.Parameters.AddWithValue("@XepLoai", xepLoai);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch { return false; }
            }
        }

        public bool DeleteDiemRenLuyen(int id)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_DeleteDiemRenLuyen", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ID", id);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch { return false; }
            }
        }

        // ==========================================
        // DỊCH VỤ HÀNH CHÍNH
        // ==========================================

        public List<YeuCauHanhChinh> GetAllYeuCau()
        {
            List<YeuCauHanhChinh> list = new List<YeuCauHanhChinh>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_GetAllYeuCauHanhChinh", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new YeuCauHanhChinh
                        {
                            MaYC = Convert.ToInt32(reader["MaYC"]),
                            MaSV = reader["MaSV"].ToString(),
                            HoTen = reader["HoTen"].ToString(),
                            LoaiDichVu = reader["LoaiDichVu"].ToString(),
                            SoLuong = reader["SoLuong"] != DBNull.Value ? Convert.ToInt32(reader["SoLuong"]) : 1,
                            MoTa = reader["MoTa"].ToString(),
                            NgayGui = Convert.ToDateTime(reader["NgayGui"]),
                            TrangThai = reader["TrangThai"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public bool UpdateTrangThaiYeuCau(int maYC, string trangThai)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_UpdateTrangThaiYeuCau", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MaYC", maYC);
                    cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch { return false; }
            }
        }
    }
}
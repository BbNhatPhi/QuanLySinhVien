using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using StudentManagementSystem.Models;
namespace StudentManagementSystem.DAO
{
    public class TeacherDAO
    {
        private string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // 1. Lấy lịch dạy của Giảng viên (Đã kết nối với bảng ThoiKhoaBieu thực tế)
        public List<LichDay> GetLichDay(string username)
        {
            List<LichDay> list = new List<LichDay>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
                    SELECT lhp.MaLopHP, m.TenMon, tkb.PhongHoc, tkb.NgayHoc, tkb.CaHoc
                    FROM Users u
                    INNER JOIN GiangVien gv ON u.UserID = gv.UserID
                    INNER JOIN LopHocPhan lhp ON gv.MaGV = lhp.MaGV
                    INNER JOIN ThoiKhoaBieu tkb ON lhp.MaLopHP = tkb.MaLopHP
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

        // 2. Lấy danh sách điểm
        public List<SinhVienDiem> GetDanhSachDiem(string maLopHP)
        {
            List<SinhVienDiem> list = new List<SinhVienDiem>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
                    SELECT sv.MaSV, sv.HoTen, d.DiemCC, d.DiemGK, d.DiemCK, d.DiemTong
                    FROM Diem d
                    INNER JOIN SinhVien sv ON d.MaSV = sv.MaSV
                    WHERE d.MaLopHP = @MaLopHP
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

        // 3. Cập nhật điểm cho Sinh viên
        public void UpdateDiem(string maSV, string maLopHP, double diemCC, double diemGK, double diemCK)
        {
            double diemTong = (diemCC * 0.1) + (diemGK * 0.3) + (diemCK * 0.6);
            diemTong = Math.Round(diemTong, 1);

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
                    UPDATE Diem 
                    SET DiemCC = @CC, DiemGK = @GK, DiemCK = @CK, DiemTong = @Tong
                    WHERE MaSV = @MaSV AND MaLopHP = @MaLopHP";

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

        // 4. Lấy danh sách chi tiết sinh viên (Phục vụ Điểm danh)
        public List<SinhVienLop> GetDanhSachSinhVien(string maLopHP)
        {
            List<SinhVienLop> list = new List<SinhVienLop>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"
                    SELECT sv.MaSV, sv.HoTen, sv.GioiTinh, sv.NgaySinh, sv.Email, d.DiemCC
                    FROM Diem d
                    INNER JOIN SinhVien sv ON d.MaSV = sv.MaSV
                    WHERE d.MaLopHP = @MaLopHP
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

        // 5. Hàm trừ điểm chuyên cần khi vắng mặt
        public void TruDiemChuyenCan(string maSV, string maLopHP)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = @"UPDATE Diem SET DiemCC = CASE WHEN DiemCC >= 1 THEN DiemCC - 1 ELSE 0 END 
                               WHERE MaSV = @MaSV AND MaLopHP = @MaLopHP";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaSV", maSV);
                cmd.Parameters.AddWithValue("@MaLopHP", maLopHP);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
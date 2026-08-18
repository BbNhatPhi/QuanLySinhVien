using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using StudentManagementSystem.Models;
using StudentManagementSystem.Utils; // Dùng để gọi SecurityHelper
using System.Data;
namespace StudentManagementSystem.DAO
{
    public class AdminDAO
    {
        private string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // ==========================================
        // 1. DASHBOARD & THỐNG KÊ
        // ==========================================
        public Dictionary<string, int> GetDashboardStats()
        {
            var stats = new Dictionary<string, int>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_GetDashboardStats", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        stats.Add("Students", Convert.ToInt32(reader["TotalStudents"]));
                        stats.Add("Teachers", Convert.ToInt32(reader["TotalTeachers"]));
                        stats.Add("Khoa", Convert.ToInt32(reader["TotalKhoa"]));
                        stats.Add("ActiveUsers", Convert.ToInt32(reader["ActiveUsers"]));
                    }
                }
            }
            return stats;
        }

        // ==========================================
        // 2. QUẢN LÝ TÀI KHOẢN (USERS)
        // ==========================================
        public List<User> GetAllUsers()
        {
            List<User> list = new List<User>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_GetAllUsersList", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new User
                        {
                            UserID = Convert.ToInt32(reader["UserID"]),
                            Username = reader["Username"].ToString(),
                            RoleID = Convert.ToInt32(reader["RoleID"]),
                            RoleName = reader["RoleName"].ToString(),
                            IsActive = Convert.ToBoolean(reader["IsActive"]),
                            HoTen = reader["HoTen"] != DBNull.Value ? reader["HoTen"].ToString() : "Chưa cập nhật",
                            Email = reader["Email"].ToString(),


                        });
                    }
                }
            }
            return list;
        }

        public bool CreateUser(string username, string rawPassword, int roleId, string hoTen)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                // Mã hóa mật khẩu như cũ
                string hash = SecurityHelper.HashPassword(rawPassword);

                SqlCommand cmd = new SqlCommand("sp_CreateUser", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@PasswordHash", hash);
                cmd.Parameters.AddWithValue("@RoleID", roleId);

                // Bổ sung thêm dòng này để truyền Họ Tên xuống SQL Server
                cmd.Parameters.AddWithValue("@HoTen", hoTen);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool ToggleUserStatus(int userId, bool isActive)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_ToggleUserStatus", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.Parameters.AddWithValue("@IsActive", isActive ? 1 : 0);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ==========================================
        // 3. QUẢN LÝ KHOA
        // ==========================================
        public List<Khoa> GetAllKhoa()
        {
            List<Khoa> list = new List<Khoa>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_GetAllKhoaList", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Khoa
                        {
                            KhoaID = Convert.ToInt32(reader["KhoaID"]),
                            TenKhoa = reader["TenKhoa"].ToString(),
                            LienHe = reader["LienHe"] != DBNull.Value ? reader["LienHe"].ToString() : ""
                        });
                    }
                }
            }
            return list;
        }

        public void AddKhoa(string tenKhoa, string lienHe)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_AddKhoaAdmin", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TenKhoa", tenKhoa);
                cmd.Parameters.AddWithValue("@LienHe", string.IsNullOrEmpty(lienHe) ? (object)DBNull.Value : lienHe);

                // THÊM KHAI BÁO NÀY ĐỂ HỨNG @Message TỪ SQL SERVER
                SqlParameter msgParam = new SqlParameter("@Message", SqlDbType.NVarChar, 255);
                msgParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(msgParam);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // ==========================================
        // 4. QUẢN LÝ NGÀNH
        // ==========================================
        public List<Nganh> GetAllNganh()
        {
            List<Nganh> list = new List<Nganh>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_GetAllNganhList", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Nganh
                        {
                            NganhID = Convert.ToInt32(reader["NganhID"]),
                            TenNganh = reader["TenNganh"].ToString(),
                            KhoaID = Convert.ToInt32(reader["KhoaID"]),
                            TenKhoa = reader["TenKhoa"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public void AddNganh(string tenNganh, int khoaId)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_AddNganhAdmin", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@TenNganh", tenNganh);
                cmd.Parameters.AddWithValue("@KhoaID", khoaId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public bool DeleteUser(int userId)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("DELETE FROM Users WHERE UserID = @Id", conn);
                    cmd.Parameters.AddWithValue("@Id", userId);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch
                {
                    // Sẽ nhảy vào đây nếu tài khoản đang bị dính khóa ngoại (đã có hồ sơ sinh viên/giảng viên)
                    return false;
                }
            }
        }

        public bool ChangeUserRole(int userId, int roleId)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_UpdateUserRole", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.Parameters.AddWithValue("@RoleID", roleId);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        public bool UpdateNganh(int nganhId, string tenNganh, int khoaId)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_UpdateNganhAdmin", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@NganhID", nganhId);
                    cmd.Parameters.AddWithValue("@TenNganh", tenNganh);
                    cmd.Parameters.AddWithValue("@KhoaID", khoaId);

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
        // 5. BACKUP DỮ LIỆU
        // ==========================================
        public string BackupDatabase()
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_BackupDatabase", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter pathParam = new SqlParameter("@BackupPath", SqlDbType.NVarChar, 255);
                pathParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(pathParam);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    string result = pathParam.Value.ToString();

                    if (result.StartsWith("ERROR:"))
                        return result; // Trả về lỗi nếu SQL báo lỗi
                    else
                        return "SUCCESS|" + result; // Trả về đường dẫn nếu thành công
                }
                catch (Exception ex)
                {
                    return "ERROR: " + ex.Message;
                }
            }
        }
        // ==========================================
        // QUẢN LÝ PHÒNG HỌC
        // ==========================================

        public List<PhongHoc> GetAllPhongHoc()
        {
            List<PhongHoc> list = new List<PhongHoc>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_GetAllPhongHoc", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new PhongHoc
                        {
                            MaPhong = reader["MaPhong"].ToString(),
                            SucChua = Convert.ToInt32(reader["SucChua"]),
                            LoaiPhong = reader["LoaiPhong"].ToString(),
                            TinhTrang = reader["TinhTrang"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public bool AddPhongHoc(string maPhong, int sucChua, string loaiPhong, string tinhTrang)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_AddPhongHoc", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MaPhong", maPhong);
                    cmd.Parameters.AddWithValue("@SucChua", sucChua);
                    cmd.Parameters.AddWithValue("@LoaiPhong", loaiPhong);
                    cmd.Parameters.AddWithValue("@TinhTrang", tinhTrang);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch { return false; } // Trùng mã phòng
            }
        }

        public bool UpdatePhongHoc(string maPhong, int sucChua, string loaiPhong, string tinhTrang)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_UpdatePhongHoc", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MaPhong", maPhong);
                    cmd.Parameters.AddWithValue("@SucChua", sucChua);
                    cmd.Parameters.AddWithValue("@LoaiPhong", loaiPhong);
                    cmd.Parameters.AddWithValue("@TinhTrang", tinhTrang);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch { return false; }
            }
        }

        public bool DeletePhongHoc(string maPhong)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_DeletePhongHoc", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MaPhong", maPhong);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch { return false; } // Lỗi khi phòng đang được dùng để xếp lịch
            }
        }
    }
}
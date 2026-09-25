using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using StudentManagementSystem.Models;
using System.Data;
namespace StudentManagementSystem.DAO
{
    public class UserDAO
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public User GetUserByUsername(string username)
        {
            User user = null;
            // Dùng USING để tự động đóng và giải phóng kết nối
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Câu query SQL được tách biệt riêng
                string sql = @"SELECT u.UserID, u.Username, u.PasswordHash, u.RoleID, r.RoleName, u.HoTen, u.Email 
                               FROM Users u
                               INNER JOIN Roles r ON u.RoleID = r.RoleID
                               WHERE u.Username = @Username AND u.IsActive = 1";

                SqlCommand cmd = new SqlCommand(sql, conn);
                // Dùng Parameter để chống SQL Injection
                cmd.Parameters.AddWithValue("@Username", username);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new User
                        {
                            UserID = Convert.ToInt32(reader["UserID"]),
                            Username = reader["Username"].ToString(),
                            PasswordHash = reader["PasswordHash"].ToString(),
                            RoleID = Convert.ToInt32(reader["RoleID"]),
                            RoleName = reader["RoleName"].ToString(),
                            HoTen = reader["HoTen"] != DBNull.Value ? reader["HoTen"].ToString() : string.Empty,
                            Email = reader["Email"] != DBNull.Value ? reader["Email"].ToString() : string.Empty
                        };
                    }
                }

            }
            return user;
        }
    }
}
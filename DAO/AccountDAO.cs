using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using StudentManagementSystem.Utils; // Gọi thư viện băm mật khẩu
namespace StudentManagementSystem.DAO
{
    public class AccountDAO
    {
        private string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public bool ChangePassword(string username, string newPassword)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                // Băm mật khẩu mới bằng BCrypt trước khi lưu vào database
                string hash = SecurityHelper.HashPassword(newPassword);

                SqlCommand cmd = new SqlCommand("sp_ChangePassword", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@NewPasswordHash", hash);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}
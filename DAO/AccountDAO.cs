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
            try
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
                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0) return true;

                    // Fallback SQL trực tiếp nếu SP trả về 0
                    SqlCommand cmdFallback = new SqlCommand("UPDATE Users SET PasswordHash = @Hash WHERE Username = @Username", conn);
                    cmdFallback.Parameters.AddWithValue("@Hash", hash);
                    cmdFallback.Parameters.AddWithValue("@Username", username);
                    return cmdFallback.ExecuteNonQuery() > 0;
                }
            }
            catch
            {
                // Fallback nếu gọi stored procedure gặp sự cố
                try
                {
                    using (SqlConnection conn = new SqlConnection(connStr))
                    {
                        string hash = SecurityHelper.HashPassword(newPassword);
                        SqlCommand cmd = new SqlCommand("UPDATE Users SET PasswordHash = @Hash WHERE Username = @Username", conn);
                        cmd.Parameters.AddWithValue("@Hash", hash);
                        cmd.Parameters.AddWithValue("@Username", username);
                        conn.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}

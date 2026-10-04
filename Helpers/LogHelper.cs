using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;

namespace StudentManagementSystem.Helpers
{
    public static class LogHelper
    {
        private static string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public static void Log(string action)
        {
            try
            {
                string username = HttpContext.Current.User.Identity.IsAuthenticated ? HttpContext.Current.User.Identity.Name : "System/Guest";
                string ip = HttpContext.Current.Request.UserHostAddress;

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    string sql = "INSERT INTO SystemLogs (Username, Action, IPAddress) VALUES (@User, @Action, @IP)";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@User", username);
                    cmd.Parameters.AddWithValue("@Action", action);
                    cmd.Parameters.AddWithValue("@IP", ip ?? (object)DBNull.Value);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                // Ignore log errors
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BCrypt.Net;

namespace StudentManagementSystem.Utils
{
    public static class SecurityHelper
    {
        // Hàm tạo mã băm (dùng khi đăng ký/đổi mật khẩu)
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return string.Empty;
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Hàm kiểm tra mật khẩu
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
                return false;

            // Mật khẩu tạm thời mặc định chưa băm
            if (hashedPassword == "123456")
                return password == "123456";

            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch
            {
                // Nếu hashedPassword không phải định dạng BCrypt hợp lệ
                return password == hashedPassword;
            }
        }
    }
}

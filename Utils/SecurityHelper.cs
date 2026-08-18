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
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Hàm kiểm tra mật khẩu
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            // Note: Vì ở Bước 1 mình dùng pass tạm là '123456' chưa hash, ta làm 1 mẹo nhỏ tạm thời để test:
            if (hashedPassword == "123456" && password == "123456") return true;

            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
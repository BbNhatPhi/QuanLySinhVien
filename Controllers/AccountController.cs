using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using StudentManagementSystem.DAO;
using StudentManagementSystem.Models;
using StudentManagementSystem.Utils;
namespace StudentManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private UserDAO _userDAO = new UserDAO();

        // Khai báo thêm AccountDAO để xử lý Đổi mật khẩu
        private AccountDAO _accountDAO = new AccountDAO();

        // ============ CHỐNG BRUTE-FORCE (KHÓA TẠM TÀI KHOẢN) ============
        private static readonly object _lock = new object();
        private static readonly Dictionary<string, int> _failedAttempts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, DateTime> _lockoutUntil = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);
        private const int MaxFailedAttempts = 5;
        private const int LockoutMinutes = 15;

        private bool IsLockedOut(string username)
        {
            lock (_lock)
            {
                DateTime until;
                return _lockoutUntil.TryGetValue(username, out until) && until > DateTime.Now;
            }
        }

        private void RegisterFailedLogin(string username)
        {
            lock (_lock)
            {
                int count = _failedAttempts.ContainsKey(username) ? _failedAttempts[username] : 0;
                count++;
                _failedAttempts[username] = count;
                if (count >= MaxFailedAttempts)
                {
                    _lockoutUntil[username] = DateTime.Now.AddMinutes(LockoutMinutes);
                    _failedAttempts[username] = 0;
                }
            }
        }

        private void ResetFailedLogin(string username)
        {
            lock (_lock)
            {
                _failedAttempts.Remove(username);
                _lockoutUntil.Remove(username);
            }
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            if (!string.IsNullOrEmpty(username) && IsLockedOut(username))
            {
                ViewBag.Error = string.Format("Tài khoản đã bị khóa tạm thời do nhập sai quá nhiều lần. Vui lòng thử lại sau {0} phút!", LockoutMinutes);
                return View();
            }

            User user = _userDAO.GetUserByUsername(username);

            if (user != null && SecurityHelper.VerifyPassword(password, user.PasswordHash))
            {
                ResetFailedLogin(username);

                // Kiểm tra xem tài khoản có đang dùng mật khẩu mặc định 123456 không
                bool isDefaultPassword = (user.PasswordHash == "123456" || password == "123456");

                // FIX LỖI LOGIC: Lấy đúng thời gian timeout cấu hình trong Web.config (2880 phút = 48 tiếng)
                // thay vì hard-code 60 phút như trước (khiến người dùng bị đăng xuất sớm hơn cấu hình rất nhiều).
                double timeoutMinutes = FormsAuthentication.Timeout.TotalMinutes;

                // 1. Khởi tạo vé chứng thực (Authentication Ticket)
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                    1,
                    user.Username,
                    DateTime.Now,
                    DateTime.Now.AddMinutes(timeoutMinutes),
                    false,
                    user.RoleName // Cất RoleName vào UserData của Ticket để làm Phân quyền
                );

                // 2. Mã hóa vé và lưu vào Cookie (đặt hạn cookie khớp hạn vé để giữ đăng nhập đủ 48 giờ)
                string encryptedTicket = FormsAuthentication.Encrypt(ticket);
                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                cookie.Expires = ticket.Expiration;
                Response.Cookies.Add(cookie);

                // Nếu là mật khẩu mặc định 123456, đánh dấu cờ cảnh báo trong Session
                if (isDefaultPassword)
                {
                    Session["MustChangePassword"] = true;
                }

                // 3. Điều hướng theo phân quyền (Role-based Routing)
                switch (user.RoleName)
                {
                    case "Admin": return RedirectToAction("Index", "Admin");
                    case "GiangVien": return RedirectToAction("Index", "Teacher");
                    case "NhanVien": return RedirectToAction("Index", "Staff");
                    case "SinhVien": return RedirectToAction("Index", "Student");
                    default: return RedirectToAction("Index", "Home");
                }
            }

            if (!string.IsNullOrEmpty(username))
            {
                RegisterFailedLogin(username);
            }

            ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng!";
            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        // ==========================================
        // TÍNH NĂNG ĐỔI MẬT KHẨU
        // ==========================================

        // GET: /Account/ChangePassword (Hiển thị trang đổi mật khẩu)
        [Authorize] // Bắt buộc phải đăng nhập mới được vào trang này
        public ActionResult ChangePassword()
        {
            string username = User.Identity.Name;
            User user = _userDAO.GetUserByUsername(username);
            ViewBag.UserInfo = user;
            return View();
        }

        // POST: Xử lý khi người dùng bấm nút Lưu
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            string username = User.Identity.Name;
            User currentUser = _userDAO.GetUserByUsername(username);
            ViewBag.UserInfo = currentUser;

            if (string.IsNullOrWhiteSpace(currentPassword))
            {
                TempData["ErrorMsg"] = "Vui lòng nhập mật khẩu hiện tại!";
                return RedirectToAction("ChangePassword");
            }

            // Xác thực mật khẩu hiện tại trước khi cho phép đổi
            if (currentUser == null || !SecurityHelper.VerifyPassword(currentPassword, currentUser.PasswordHash))
            {
                TempData["ErrorMsg"] = "Mật khẩu hiện tại không chính xác!";
                return RedirectToAction("ChangePassword");
            }

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            {
                TempData["ErrorMsg"] = "Mật khẩu mới phải có ít nhất 6 ký tự!";
                return RedirectToAction("ChangePassword");
            }

            if (newPassword == currentPassword)
            {
                TempData["ErrorMsg"] = "Mật khẩu mới không được trùng với mật khẩu hiện tại!";
                return RedirectToAction("ChangePassword");
            }

            if (newPassword != confirmPassword)
            {
                TempData["ErrorMsg"] = "Mật khẩu xác nhận không khớp! Vui lòng nhập lại.";
                return RedirectToAction("ChangePassword");
            }

            bool isSuccess = _accountDAO.ChangePassword(username, newPassword);

            if (isSuccess)
            {
                Session.Remove("MustChangePassword");
                TempData["SuccessMsg"] = "Đổi mật khẩu thành công! Mật khẩu mới đã được cập nhật an toàn vào hệ thống.";
            }
            else
            {
                TempData["ErrorMsg"] = "Đã xảy ra lỗi hệ thống khi cập nhật mật khẩu, vui lòng thử lại sau!";
            }

            return RedirectToAction("ChangePassword");
        }
    
        // ==========================================
        // QUÊN MẬT KHẨU & OTP (BẢO MẬT)
        // ==========================================
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string username)
        {
            var user = _userDAO.GetUserByUsername(username);
            if (user == null || !user.IsActive)
            {
                TempData["ErrorMsg"] = "Tài khoản không tồn tại hoặc đã bị khóa!";
                return View();
            }

            // Sinh mã OTP 6 số ngẫu nhiên
            Random rnd = new Random();
            string otp = rnd.Next(100000, 999999).ToString();

            // Lưu OTP và Username vào Session (có thể lưu DB nếu làm thực tế)
            Session["OTP"] = otp;
            Session["ResetUser"] = username;
            Session["OTPExpiry"] = DateTime.Now.AddMinutes(5); // OTP có hiệu lực 5 phút

            // Ghi Log vào hệ thống và Ghi ra file text (Giả lập gửi Email)
            StudentManagementSystem.Helpers.LogHelper.Log($"Yêu cầu cấp lại mật khẩu cho tài khoản {username}. OTP được sinh ra.");
            
            string appDataPath = Server.MapPath("~/App_Data");
            if (!System.IO.Directory.Exists(appDataPath)) System.IO.Directory.CreateDirectory(appDataPath);
            string emailMock = $@"[MÔ PHỎNG EMAIL]
To: {username}@student.ntu.edu.vn
Subject: Mã OTP cấp lại mật khẩu

Mã OTP của bạn là: {otp}
Vui lòng không chia sẻ mã này cho bất kỳ ai. Mã có hiệu lực 5 phút.";
            System.IO.File.WriteAllText(System.IO.Path.Combine(appDataPath, $"EmailOTP_{username}.txt"), emailMock);

            // Chuyển hướng sang trang nhập OTP
            return RedirectToAction("VerifyOTP");
        }

        [HttpGet]
        public ActionResult VerifyOTP()
        {
            if (Session["ResetUser"] == null) return RedirectToAction("Login");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyOTP(string otp)
        {
            if (Session["ResetUser"] == null || Session["OTP"] == null || Session["OTPExpiry"] == null)
            {
                TempData["ErrorMsg"] = "Phiên giao dịch đã hết hạn. Vui lòng thử lại!";
                return RedirectToAction("ForgotPassword");
            }

            DateTime expiry = (DateTime)Session["OTPExpiry"];
            if (DateTime.Now > expiry)
            {
                TempData["ErrorMsg"] = "Mã OTP đã hết hạn!";
                return View();
            }

            string realOtp = Session["OTP"].ToString();
            if (otp == realOtp)
            {
                // Cho phép đổi mật khẩu
                Session["OTPVerified"] = true;
                return RedirectToAction("ResetPassword");
            }

            TempData["ErrorMsg"] = "Mã OTP không chính xác!";
            return View();
        }

        [HttpGet]
        public ActionResult ResetPassword()
        {
            if (Session["OTPVerified"] == null || !(bool)Session["OTPVerified"]) return RedirectToAction("Login");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(string newPassword, string confirmPassword)
        {
            if (Session["ResetUser"] == null || Session["OTPVerified"] == null || !(bool)Session["OTPVerified"]) return RedirectToAction("Login");

            if (newPassword != confirmPassword)
            {
                TempData["ErrorMsg"] = "Mật khẩu xác nhận không khớp!";
                return View();
            }

            string username = Session["ResetUser"].ToString();
            string hashedPassword = SecurityHelper.HashPassword(newPassword);

            // Cập nhật Database
            bool success = _accountDAO.ChangePassword(username, hashedPassword);
            if (success)
            {
                StudentManagementSystem.Helpers.LogHelper.Log($"Tài khoản {username} đã đặt lại mật khẩu thành công qua OTP.");
                TempData["SuccessMsg"] = "Đặt lại mật khẩu thành công! Bạn có thể đăng nhập ngay bây giờ.";
                Session.Clear(); // Xóa Session
                return RedirectToAction("Login");
            }

            TempData["ErrorMsg"] = "Lỗi hệ thống khi cập nhật mật khẩu!";
            return View();
        }
}
}

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

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            User user = _userDAO.GetUserByUsername(username);

            if (user != null && SecurityHelper.VerifyPassword(password, user.PasswordHash))
            {
                // 1. Khởi tạo vé chứng thực (Authentication Ticket)
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                    1,
                    user.Username,
                    DateTime.Now,
                    DateTime.Now.AddMinutes(60), // Hết hạn sau 60 phút
                    false,
                    user.RoleName // Cất RoleName vào UserData của Ticket để làm Phân quyền
                );

                // 2. Mã hóa vé và lưu vào Cookie
                string encryptedTicket = FormsAuthentication.Encrypt(ticket);
                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                Response.Cookies.Add(cookie);

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

            ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng!";
            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }

        // ==========================================
        // THÊM TÍNH NĂNG ĐỔI MẬT KHẨU
        // ==========================================

        // GET: /Account/ChangePassword (Hiển thị trang đổi mật khẩu)
        [Authorize] // Bắt buộc phải đăng nhập mới được vào trang này
        public ActionResult ChangePassword()
        {
            return View();
        }

        // POST: Xử lý khi người dùng bấm nút Lưu
        [HttpPost]
        [Authorize]
        public ActionResult ChangePassword(string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                TempData["ErrorMsg"] = "Mật khẩu xác nhận không khớp! Vui lòng nhập lại.";
                return RedirectToAction("ChangePassword");
            }

            // Lấy tên tài khoản (Username) của người đang đăng nhập hiện tại
            string username = User.Identity.Name;

            bool isSuccess = _accountDAO.ChangePassword(username, newPassword);

            if (isSuccess)
            {
                TempData["SuccessMsg"] = "Đổi mật khẩu thành công! Ở lần đăng nhập tiếp theo, hãy sử dụng mật khẩu mới này.";
            }
            else
            {
                TempData["ErrorMsg"] = "Đã xảy ra lỗi hệ thống, vui lòng thử lại sau!";
            }

            return RedirectToAction("ChangePassword");
        }
    }
}
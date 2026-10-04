import codecs
import re

with codecs.open('Controllers/AccountController.cs', 'r', 'utf-8-sig') as f:
    text = f.read()

forgot_logic = '''
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
            string emailMock = $"[MÔ PHỎNG EMAIL]\nTo: {username}@student.ntu.edu.vn\nSubject: Mã OTP cấp lại mật khẩu\n\nMã OTP của bạn là: {otp}\nVui lòng không chia sẻ mã này cho bất kỳ ai. Mã có hiệu lực 5 phút.";
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
            string hashedPassword = PasswordHasher.HashPassword(newPassword);

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
'''

if 'ForgotPassword' not in text:
    idx = text.rfind('}')
    idx = text.rfind('}', 0, idx)
    text = text[:idx] + forgot_logic + text[idx:]
    with codecs.open('Controllers/AccountController.cs', 'w', 'utf-8-sig') as f:
        f.write(text)
    print("Added OTP logic to AccountController")
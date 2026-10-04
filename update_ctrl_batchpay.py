import codecs
import re

with codecs.open('Controllers/StudentController.cs', 'r', 'utf-8-sig') as f:
    text = f.read()

# Update ThanhToan action to handle multiple subjects
old_thanhtoan = '''        public ActionResult ThanhToan(string maLopHP, string tenMon, double soTien)
        {
            ViewBag.MaLopHP = maLopHP;
            ViewBag.TenMon = tenMon;
            return View(lichThi);
        }'''

new_thanhtoan = '''        public ActionResult ThanhToan(string maLopHP, string tenMon, double soTien)
        {
            ViewBag.MaLopHP = maLopHP;
            
            // Nếu có nhiều môn (phân cách bằng dấu phẩy)
            if (maLopHP.Contains(","))
            {
                ViewBag.TenMon = "Thanh toán nhiều học phần";
            }
            else
            {
                ViewBag.TenMon = tenMon;
            }
            
            ViewBag.SoTien = soTien;
            return View();
        }'''

if 'if (maLopHP.Contains(","))' not in text:
    text = text.replace(old_thanhtoan, new_thanhtoan)

# Update XacNhanThanhToan to handle multiple subjects
old_xacnhan = '''        public ActionResult XacNhanThanhToan(string maLopHP, double soTien)
        {
            // Lấy tài khoản sinh viên đang đăng nhập
            string username = User.Identity.Name;

            // Gọi DAO để cập nhật trạng thái môn này thành "Đã thanh toán"
            // FIX LỖI LOGIC: Kiểm tra kết quả trả về, không báo thành công khi giao dịch thất bại
            bool isSuccess = _studentDAO.ThanhToanHocPhi(username, maLopHP);

            if (isSuccess)
            {
                TempData["SuccessMsg"] = $"Giao dịch thành công! Bạn đã nộp {soTien:N0} VNĐ cho học phần {maLopHP}.";
            }
            else
            {
                TempData["ErrorMsg"] = "Giao dịch thất bại! Có lỗi xảy ra trong quá trình thanh toán.";
            }

            return RedirectToAction("XemHocPhi");
        }'''

new_xacnhan = '''        public ActionResult XacNhanThanhToan(string maLopHP, double soTien)
        {
            string username = User.Identity.Name;
            bool allSuccess = true;
            
            // Xử lý thanh toán nhiều môn cùng lúc (được phân cách bởi dấu phẩy)
            string[] maLopList = maLopHP.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach(var ma in maLopList)
            {
                bool isSuccess = _studentDAO.ThanhToanHocPhi(username, ma.Trim());
                if (!isSuccess) allSuccess = false;
            }

            if (allSuccess)
            {
                StudentManagementSystem.Helpers.LogHelper.Log($"Sinh viên thanh toán thành công {soTien:N0} VNĐ cho {maLopList.Length} môn: {maLopHP}");
                TempData["SuccessMsg"] = $"Giao dịch thành công! Bạn đã thanh toán {soTien:N0} VNĐ cho {maLopList.Length} học phần.";
            }
            else
            {
                TempData["ErrorMsg"] = "Có lỗi xảy ra trong quá trình thanh toán một số môn!";
            }

            return RedirectToAction("XemHocPhi");
        }'''

if 'string[] maLopList = maLopHP.Split' not in text:
    text = text.replace(old_xacnhan, new_xacnhan)

with codecs.open('Controllers/StudentController.cs', 'w', 'utf-8-sig') as f:
    f.write(text)
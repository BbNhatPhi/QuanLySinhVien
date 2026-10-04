import codecs
import re

with codecs.open('Controllers/StudentController.cs', 'r', 'utf-8-sig') as f:
    text = f.read()

# Replace ThanhToan completely
pattern = re.compile(r'public ActionResult ThanhToan\(string maLopHP, string tenMon, double soTien\).*?\{.*?return View\(.*?\);.*?\}', re.DOTALL)
new_thanhtoan = '''public ActionResult ThanhToan(string maLopHP, string tenMon, double soTien)
        {
            ViewBag.MaLopHP = maLopHP;
            
            // Nếu có nhiều môn (phân cách bằng dấu phẩy)
            if (maLopHP != null && maLopHP.Contains(","))
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
text = re.sub(pattern, new_thanhtoan, text)

# Replace XacNhanThanhToan completely
pattern2 = re.compile(r'public ActionResult XacNhanThanhToan\(string maLopHP, double soTien\).*?return RedirectToAction\("XemHocPhi"\);\s*\}', re.DOTALL)
new_xacnhan = '''public ActionResult XacNhanThanhToan(string maLopHP, double soTien)
        {
            string username = User.Identity.Name;
            bool allSuccess = true;
            
            // Xử lý thanh toán nhiều môn cùng lúc
            string[] maLopList = maLopHP.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach(var ma in maLopList)
            {
                bool isSuccess = _studentDAO.ThanhToanHocPhi(username, ma.Trim());
                if (!isSuccess) allSuccess = false;
            }

            if (allSuccess)
            {
                StudentManagementSystem.Helpers.LogHelper.Log($"Sinh viên thanh toán thành công {soTien:N0} VNĐ cho {maLopList.Length} môn.");
                TempData["SuccessMsg"] = $"Giao dịch thành công! Bạn đã thanh toán {soTien:N0} VNĐ cho {maLopList.Length} học phần.";
            }
            else
            {
                TempData["ErrorMsg"] = "Có lỗi xảy ra trong quá trình thanh toán một số môn!";
            }

            return RedirectToAction("XemHocPhi");
        }'''
text = re.sub(pattern2, new_xacnhan, text)

with codecs.open('Controllers/StudentController.cs', 'w', 'utf-8-sig') as f:
    f.write(text)
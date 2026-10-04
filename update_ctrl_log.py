import codecs
import re

with codecs.open('Controllers/StudentController.cs', 'r', 'utf-8-sig') as f:
    text = f.read()

# Add using StudentManagementSystem.Helpers if missing
if 'using StudentManagementSystem.Helpers;' not in text:
    text = text.replace('using System.Web.Mvc;', 'using System.Web.Mvc;\nusing StudentManagementSystem.Helpers;')

# Inject logging into XuLyDangKy
if 'LogHelper.Log' not in text:
    old_dangky = '''        public ActionResult XuLyDangKy(string maLopHP)
        {
            // Lấy tên đăng nhập của sinh viên hiện tại (chính là Mã Sinh Viên)
            string username = User.Identity.Name;

            string result = _studentDAO.DangKyLop(username, maLopHP);

            if (result == "Success")
            {
                TempData["SuccessMsg"] = $"Chúc mừng! Đăng ký thành công lớp {maLopHP}.";
            }'''
            
    new_dangky = '''        public ActionResult XuLyDangKy(string maLopHP)
        {
            // Lấy tên đăng nhập của sinh viên hiện tại (chính là Mã Sinh Viên)
            string username = User.Identity.Name;

            string result = _studentDAO.DangKyLop(username, maLopHP);

            if (result == "Success")
            {
                LogHelper.Log($"Sinh viên đăng ký lớp học phần: {maLopHP}");
                TempData["SuccessMsg"] = $"Chúc mừng! Đăng ký thành công lớp {maLopHP}.";
            }'''
    text = text.replace(old_dangky, new_dangky)

    old_huy = '''        public ActionResult HuyDangKy(string maLopHP)
        {
            string username = User.Identity.Name;

            string result = _studentDAO.HuyDangKyLop(username, maLopHP);

            if (result == "Success")
            {
                TempData["SuccessMsg"] = $"Đã hủy đăng ký thành công lớp {maLopHP}!";
            }'''
            
    new_huy = '''        public ActionResult HuyDangKy(string maLopHP)
        {
            string username = User.Identity.Name;

            string result = _studentDAO.HuyDangKyLop(username, maLopHP);

            if (result == "Success")
            {
                LogHelper.Log($"Sinh viên hủy lớp học phần: {maLopHP}");
                TempData["SuccessMsg"] = $"Đã hủy đăng ký thành công lớp {maLopHP}!";
            }'''
    text = text.replace(old_huy, new_huy)

with codecs.open('Controllers/StudentController.cs', 'w', 'utf-8-sig') as f:
    f.write(text)
print("Updated StudentController with LogHelper")
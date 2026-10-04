import codecs
import re

with codecs.open('Controllers/StaffController.cs', 'r', 'utf-8-sig') as f:
    text = f.read()

old_add = '''        public ActionResult AddLichThi(string maLopHP, DateTime ngayThi, int caThi, string phongThi)
        {
            string result = _staffDAO.AddLichThi(maLopHP, ngayThi, caThi, phongThi);
            if (result == "Success") TempData["SuccessMsg"] = "Đã lên lịch thi thành công!";
            else TempData["ErrorMsg"] = result;'''

new_add = '''        public ActionResult AddLichThi(string maLopHP, DateTime ngayThi, int caThi, string phongThi)
        {
            string result = _staffDAO.AddLichThi(maLopHP, ngayThi, caThi, phongThi);
            if (result == "Success") {
                StudentManagementSystem.Helpers.LogHelper.Log($"Giáo vụ xếp lịch thi cho lớp {maLopHP}: {ngayThi.ToString("dd/MM/yyyy")} Ca {caThi} tại {phongThi}");
                TempData["SuccessMsg"] = "Đã lên lịch thi thành công!";
            } else TempData["ErrorMsg"] = result;'''

old_edit = '''        public ActionResult EditLichThi(string maLopHP, DateTime ngayThi, int caThi, string phongThi)
        {
            string result = _staffDAO.UpdateLichThi(maLopHP, ngayThi, caThi, phongThi);
            if (result == "Success") TempData["SuccessMsg"] = "Đã cập nhật lịch thi thành công!";
            else TempData["ErrorMsg"] = result;'''

new_edit = '''        public ActionResult EditLichThi(string maLopHP, DateTime ngayThi, int caThi, string phongThi)
        {
            string result = _staffDAO.UpdateLichThi(maLopHP, ngayThi, caThi, phongThi);
            if (result == "Success") {
                StudentManagementSystem.Helpers.LogHelper.Log($"Giáo vụ cập nhật lịch thi lớp {maLopHP}: {ngayThi.ToString("dd/MM/yyyy")} Ca {caThi} tại {phongThi}");
                TempData["SuccessMsg"] = "Đã cập nhật lịch thi thành công!";
            } else TempData["ErrorMsg"] = result;'''

text = text.replace(old_add, new_add)
text = text.replace(old_edit, new_edit)

with codecs.open('Controllers/StaffController.cs', 'w', 'utf-8-sig') as f:
    f.write(text)
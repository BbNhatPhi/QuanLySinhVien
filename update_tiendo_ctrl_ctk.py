import codecs

with codecs.open('Controllers/StudentController.cs', 'r', 'utf-8-sig') as f:
    text = f.read()

import re
old_action = r'''        public ActionResult TienDoHocTap\(\)
        \{
            string username = User\.Identity\.Name;
            var tienDo = _studentDAO\.GetTienDoHocTap\(username\);
            ViewBag\.BangDiem = _studentDAO\.GetBangDiem\(username\);
            ViewBag\.ThongTin = _studentDAO\.GetThongTinCaNhan\(username\);
            return View\(tienDo\);
        \}'''

new_action = '''        public ActionResult TienDoHocTap()
        {
            string username = User.Identity.Name;
            var tienDo = _studentDAO.GetTienDoHocTap(username);
            ViewBag.ChuongTrinhKhung = _studentDAO.GetChuongTrinhKhung(username);
            ViewBag.ThongTin = _studentDAO.GetThongTinCaNhan(username);
            return View(tienDo);
        }'''

text = re.sub(old_action, new_action, text)

with codecs.open('Controllers/StudentController.cs', 'w', 'utf-8-sig') as f:
    f.write(text)

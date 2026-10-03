import codecs
import re

with codecs.open('Controllers/StudentController.cs', 'r', 'utf-8-sig') as f:
    text = f.read()

# Replace the TienDoHocTap action reliably
pattern = r'public ActionResult TienDoHocTap\(\)\s*\{\s*string username = User\.Identity\.Name;\s*var tienDo = _studentDAO\.GetTienDoHocTap\(username\);\s*return View\(tienDo\);\s*\}'
replacement = '''public ActionResult TienDoHocTap()
        {
            string username = User.Identity.Name;
            var tienDo = _studentDAO.GetTienDoHocTap(username);
            ViewBag.ChuongTrinhKhung = _studentDAO.GetChuongTrinhKhung(username);
            ViewBag.ThongTin = _studentDAO.GetThongTinCaNhan(username);
            return View(tienDo);
        }'''

text = re.sub(pattern, replacement, text)

with codecs.open('Controllers/StudentController.cs', 'w', 'utf-8-sig') as f:
    f.write(text)

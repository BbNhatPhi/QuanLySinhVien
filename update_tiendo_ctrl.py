import codecs

with codecs.open('Controllers/StudentController.cs', 'r', 'utf-8-sig') as f:
    text = f.read()

old_action = u'''        public ActionResult TienDoHocTap()
        {
            string username = User.Identity.Name;
            var tienDo = _studentDAO.GetTienDoHocTap(username);
            return View(tienDo);
        }'''

new_action = u'''        public ActionResult TienDoHocTap()
        {
            string username = User.Identity.Name;
            var tienDo = _studentDAO.GetTienDoHocTap(username);
            ViewBag.BangDiem = _studentDAO.GetBangDiem(username);
            ViewBag.ThongTin = _studentDAO.GetThongTinCaNhan(username);
            return View(tienDo);
        }'''

text = text.replace(old_action, new_action)

with codecs.open('Controllers/StudentController.cs', 'w', 'utf-8-sig') as f:
    f.write(text)

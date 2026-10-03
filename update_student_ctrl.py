import codecs
with codecs.open('Controllers/StudentController.cs', 'r', 'utf-8-sig') as f:
    text = f.read()

# Replace the Index method
old_index = u'''        // GET: /Student/Index (M?c d?nh vo s? th?y Th?i kha bi?u)
        public ActionResult Index()
        {
            string username = User.Identity.Name;
            var tkb = _studentDAO.GetTKB(username);
            return View(tkb);
        }'''

# If exact old_index string match fails due to encoding, use a regex or manual slicing.
import re
text = re.sub(
    r'public ActionResult Index\(\)[\s\S]*?return View\(.*?\);[\s\S]*?\}',
    u'''public ActionResult Index()
        {
            string username = User.Identity.Name;
            
            ViewBag.ThongTin = _studentDAO.GetThongTinCaNhan(username);
            ViewBag.TienDo = _studentDAO.GetTienDoHocTap(username);
            
            var tkb = _studentDAO.GetTKB(username);
            var lichThi = _studentDAO.GetLichThiSinhVien(username);
            var thongBao = _studentDAO.GetThongBao();
            var danhGia = _studentDAO.DemSoMonChuaDanhGia(username);

            ViewBag.SoLichHoc = tkb.Count;
            ViewBag.SoLichThi = lichThi.Count;
            ViewBag.SoThongBao = thongBao.Count;
            ViewBag.SoChuaDanhGia = danhGia;
            ViewBag.DanhSachMon = tkb; // Để hiển thị "Lớp học phần" ở góc dưới phải

            return View();
        }

        public ActionResult ThoiKhoaBieu()
        {
            string username = User.Identity.Name;
            var tkb = _studentDAO.GetTKB(username);
            return View("ThoiKhoaBieu", tkb);
        }''',
    text,
    count=1
)

with codecs.open('Controllers/StudentController.cs', 'w', 'utf-8-sig') as f:
    f.write(text)

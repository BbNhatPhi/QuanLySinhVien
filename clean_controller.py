import codecs
import re

with codecs.open('Controllers/StudentController.cs', 'r', 'utf-8-sig') as f:
    text = f.read()

# 1. Remove the blocking check in XemDiem
xem_diem_block = r'''            // KI.M TRA NGHI.P V.: Da d.nh gi. gi.ng vi.n h.t chua\?
            int soMonChuaDanhGia = _studentDAO\.DemSoMonChuaDanhGia\(username\);
            if \(soMonChuaDanhGia > 0\)
            \{
                TempData\["WarningMsg"\].*?;
                return RedirectToAction\("DanhGiaGiangVien"\);
            \}'''
text = re.sub(xem_diem_block, '', text, flags=re.DOTALL)

# 2. Remove the actions DanhGiaGiangVien and LuuDanhGia
action_block = r'''        // GET: /Student/DanhGiaGiangVien
        public ActionResult DanhGiaGiangVien\(\)
        \{
            string username = User\.Identity\.Name;
            var list = _studentDAO\.GetDanhSachCanDanhGia\(username\);
            return View\(list\);
        \}

        // POST: /Student/LuuDanhGia
        \[HttpPost\]
        \[ValidateAntiForgeryToken\]
        public ActionResult LuuDanhGia\(string maLopHP, int diemDanhGia, string nhanXet\)
        \{
            string username = User\.Identity\.Name;
            string result = _studentDAO\.LuuDanhGia\(username, maLopHP, diemDanhGia, nhanXet\);

            if \(result == "Success"\)
            \{
                TempData\["SuccessMsg"\] = "Đánh giá giảng viên thành công!";
            \}
            else
            \{
                TempData\["ErrorMsg"\] = result;
            \}

            return RedirectToAction\("DanhGiaGiangVien"\);
        \}'''
# Actually it's safer to just do string replacement or regex with wildcards for the method body
action_block_regex = r'\s*// GET: /Student/DanhGiaGiangVien[\s\S]*?return RedirectToAction\("DanhGiaGiangVien"\);\s*\}'
text = re.sub(action_block_regex, '', text)

# 3. Remove ViewBag assignments in Index()
text = re.sub(r'\s*var danhGia = _studentDAO\.DemSoMonChuaDanhGia\(username\);', '', text)
text = re.sub(r'\s*ViewBag\.SoChuaDanhGia = danhGia;', '', text)

with codecs.open('Controllers/StudentController.cs', 'w', 'utf-8-sig') as f:
    f.write(text)

import codecs

with codecs.open('Controllers/TeacherController.cs', 'r', 'utf-8-sig') as f:
    text = f.read()

new_actions = u'''
        // =====================================
        // TÍNH NĂNG ĐIỂM DANH MỚI (THAY THẾ DANH SÁCH SINH VIÊN CŨ)
        // =====================================
        public ActionResult DiemDanh(string maLopHP, string ngayHoc)
        {
            DateTime date = string.IsNullOrEmpty(ngayHoc) ? DateTime.Today : DateTime.Parse(ngayHoc);
            ViewBag.MaLopHP = maLopHP;
            ViewBag.NgayHoc = date.ToString("yyyy-MM-dd");
            var list = _teacherDAO.GetDiemDanh(maLopHP, date);
            return View(list);
        }

        [HttpPost]
        public JsonResult LuuDiemDanhNgay(string maLopHP, string maSV, string ngayHoc, string trangThai, string ghiChu)
        {
            try
            {
                DateTime date = DateTime.Parse(ngayHoc);
                _teacherDAO.LuuDiemDanh(maLopHP, maSV, date, trangThai, ghiChu);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public ActionResult QuanLyThongBao(string maLopTC)
        {
            if (string.IsNullOrEmpty(maLopTC)) return RedirectToAction("Index");
            ViewBag.MaLopTC = maLopTC;
            var list = _teacherDAO.GetThongBaoLop(maLopTC);
            return View(list);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult AddThongBaoLop(string maLopTC, string TieuDe, string NoiDung)
        {
            if (string.IsNullOrEmpty(maLopTC)) return RedirectToAction("Index");
            
            var tb = new ThongBaoLop {
                MaLopHP = maLopTC,
                TieuDe = TieuDe,
                NoiDung = NoiDung
            };
            _teacherDAO.AddThongBaoLop(tb);
            TempData["SuccessMsg"] = "Đăng thông báo cho lớp thành công!";
            return RedirectToAction("QuanLyThongBao", new { maLopTC = maLopTC });
        }

        public ActionResult QuanLyTaiLieu(string maLopTC)
        {
            if (string.IsNullOrEmpty(maLopTC)) return RedirectToAction("Index");
            ViewBag.MaLopTC = maLopTC;
            var list = _teacherDAO.GetTaiLieuLop(maLopTC);
            return View(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTaiLieuLop(string maLopTC, string TenTaiLieu, HttpPostedFileBase fileUpload)
        {
            if (string.IsNullOrEmpty(maLopTC)) return RedirectToAction("Index");

            if (fileUpload != null && fileUpload.ContentLength > 0)
            {
                string fileName = System.IO.Path.GetFileName(fileUpload.FileName);
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;
                string uploadPath = Server.MapPath("~/Content/Uploads/TaiLieu");
                if (!System.IO.Directory.Exists(uploadPath))
                {
                    System.IO.Directory.CreateDirectory(uploadPath);
                }
                string filePath = System.IO.Path.Combine(uploadPath, uniqueFileName);
                fileUpload.SaveAs(filePath);

                var tl = new TaiLieuLop {
                    MaLopHP = maLopTC,
                    TenTaiLieu = TenTaiLieu,
                    DuongDan = "/Content/Uploads/TaiLieu/" + uniqueFileName
                };
                _teacherDAO.AddTaiLieuLop(tl);
                TempData["SuccessMsg"] = "Tải tài liệu lên thành công!";
            }
            else
            {
                TempData["ErrorMsg"] = "Vui lòng chọn file hợp lệ.";
            }

            return RedirectToAction("QuanLyTaiLieu", new { maLopTC = maLopTC });
        }
'''

if 'LuuDiemDanhNgay' not in text:
    parts = text.rsplit('}', 2)
    if len(parts) == 3:
        text = parts[0] + new_actions + '\n    }\n}'
        with codecs.open('Controllers/TeacherController.cs', 'w', 'utf-8-sig') as f:
            f.write(text)
        print("Updated TeacherController.cs")
else:
    print("Already in TeacherController.cs")

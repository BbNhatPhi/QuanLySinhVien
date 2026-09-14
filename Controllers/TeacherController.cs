using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StudentManagementSystem.DAO;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Controllers
{
    // BẢO MẬT: Chỉ Giảng viên mới được truy cập
    [Authorize(Roles = "GiangVien")]
    public class TeacherController : Controller
    {
        private TeacherDAO _teacherDAO = new TeacherDAO();

        // ==========================================
        // PHẦN 1: CÁC MENU TRANG CHỦ & XEM LỚP
        // ==========================================

        // GET: /Teacher/Index (Xem Lịch Dạy / Thời khóa biểu)
        public ActionResult Index()
        {
            string username = User.Identity.Name;
            var lichDay = _teacherDAO.GetLichDay(username);
            return View(lichDay);
        }

        // GET: /Teacher/XemLop (Xem toàn bộ danh sách lớp)
        public ActionResult XemLop()
        {
            string username = User.Identity.Name;
            var danhSachLop = _teacherDAO.GetLichDay(username);
            return View(danhSachLop);
        }

        // ==========================================
        // PHẦN 2: CHỌN LỚP TỪ MENU BÊN TRÁI (TÁCH NÚT)
        // ==========================================

        // GET: /Teacher/ChonLopDiemDanh
        public ActionResult ChonLopDiemDanh()
        {
            ViewBag.ChucNang = "DiemDanh";
            ViewBag.Title = "Chọn Lớp Để Điểm Danh";
            return View("XemLop", _teacherDAO.GetLichDay(User.Identity.Name));
        }

        // GET: /Teacher/ChonLopNhapDiem
        public ActionResult ChonLopNhapDiem()
        {
            ViewBag.ChucNang = "NhapDiem";
            ViewBag.Title = "Chọn Lớp Để Nhập Điểm";
            return View("XemLop", _teacherDAO.GetLichDay(User.Identity.Name));
        }

        // GET: /Teacher/ChonLopInBangDiem
        public ActionResult ChonLopInBangDiem()
        {
            ViewBag.ChucNang = "InDiem";
            ViewBag.Title = "Chọn Lớp Để In Bảng Điểm";
            return View("XemLop", _teacherDAO.GetLichDay(User.Identity.Name));
        }

        public ActionResult ChonLopInDiem()
        {
            ViewBag.ChucNang = "InDiem";
            ViewBag.Title = "Chọn Lớp Để In Bảng Điểm";
            return View("XemLop", _teacherDAO.GetLichDay(User.Identity.Name));
        }

        // ==========================================
        // PHẦN 3: CÁC HÀM XỬ LÝ NGHIỆP VỤ CHÍNH
        // ==========================================

        // GET: /Teacher/DanhSachSinhVien (Xem chi tiết 1 lớp & Điểm danh)
        public ActionResult DanhSachSinhVien(string maLopTC)
        {
            if (string.IsNullOrEmpty(maLopTC)) return RedirectToAction("Index");
            ViewBag.MaLopTC = maLopTC;
            var sinhViens = _teacherDAO.GetDanhSachSinhVien(maLopTC);
            return View(sinhViens);
        }

        // ==========================================
        // HÀM LƯU ĐIỂM DANH
        [HttpPost]
        public ActionResult LuuDiemDanh(FormCollection form)
        {
            // 1. LẤY ĐÚNG TÊN BIẾN TỪ VIEW (maLopTC)
            string maLop = form["maLopTC"];

            // Mặc định học kỳ hiện tại (bạn có thể đổi theo kỳ thực tế)
            string hocKy = "HK1 (2025-2026)";

            TeacherDAO dao = new TeacherDAO();

            // 2. QUÉT TOÀN BỘ DANH SÁCH SINH VIÊN ĐƯỢC GỬI LÊN TỪ FORM
            foreach (string key in form.AllKeys)
            {
                // Chỉ xử lý các nút Checkbox Điểm danh
                if (key.StartsWith("TrangThai_"))
                {
                    // Bóc tách lấy đúng mã Sinh Viên (VD: "TrangThai_230001" -> "230001")
                    string maSV = key.Replace("TrangThai_", "");

                    // Lấy trạng thái ("Có mặt", "Có phép", "Không phép")
                    string trangThai = form[key];

                    // 3. XỬ LÝ LOGIC TRỪ ĐIỂM
                    if (trangThai == "Không phép")
                    {
                        // Vắng không phép: Trừ cả điểm Chuyên cần (Môn học) & Điểm rèn luyện
                        dao.TruDiemChuyenCan(maSV, maLop);
                        dao.TruDiemVangHoc(maSV, hocKy, trangThai);
                    }
                    else if (trangThai == "Có phép")
                    {
                        // Vắng có phép: Chỉ trừ điểm rèn luyện (theo luật chung của trường)
                        dao.TruDiemVangHoc(maSV, hocKy, trangThai);
                    }
                }
            }

            // 4. HIỂN THỊ THÔNG BÁO VÀ TẢI LẠI TRANG (trả về đúng biến maLopTC cho URL)
            TempData["Success"] = "Đã lưu kết quả điểm danh thành công!";
            return RedirectToAction("DanhSachSinhVien", new { maLopTC = maLop });
        }

        // GET: /Teacher/NhapDiem (Hiển thị Form nhập điểm)
        public ActionResult NhapDiem(string maLopTC)
        {
            if (string.IsNullOrEmpty(maLopTC)) return RedirectToAction("Index");
            ViewBag.MaLopTC = maLopTC;
            var danhSachDiem = _teacherDAO.GetDanhSachDiem(maLopTC);
            return View(danhSachDiem);
        }

        // ==========================================
        // HÀM LƯU ĐIỂM (ĐÃ ĐƯỢC FIX LỖI ÉP KIỂU SỐ THẬP PHÂN)
        // ==========================================
        [HttpPost]
        public ActionResult LuuDiem(FormCollection form, string maLopTC)
        {
            try
            {
                // Rút dữ liệu trực tiếp từ các thẻ name="..." gửi lên
                string[] maSVs = form.GetValues("MaSV");
                string[] diemCCs = form.GetValues("DiemCC");
                string[] diemGKs = form.GetValues("DiemGK");
                string[] diemCKs = form.GetValues("DiemCK");

                if (maSVs != null)
                {
                    for (int i = 0; i < maSVs.Length; i++)
                    {
                        string maSV = maSVs[i];
                        double cc = 0, gk = 0, ck = 0;

                        // Mẹo ép kiểu chống lỗi dấu chấm/phẩy của ASP.NET MVC
                        if (diemCCs != null && i < diemCCs.Length && !string.IsNullOrEmpty(diemCCs[i]))
                            double.TryParse(diemCCs[i].Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out cc);

                        if (diemGKs != null && i < diemGKs.Length && !string.IsNullOrEmpty(diemGKs[i]))
                            double.TryParse(diemGKs[i].Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out gk);

                        if (diemCKs != null && i < diemCKs.Length && !string.IsNullOrEmpty(diemCKs[i]))
                            double.TryParse(diemCKs[i].Replace(",", "."), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out ck);

                        // Ghi xuống CSDL
                        _teacherDAO.UpdateDiem(maSV, maLopTC, cc, gk, ck);
                    }
                    TempData["Success"] = "Đã lưu bảng điểm thành công! Điểm tổng kết đã tự động cập nhật.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi hệ thống: " + ex.Message;
            }

            return RedirectToAction("NhapDiem", new { maLopTC = maLopTC });
        }

        // GET: /Teacher/InBangDiem (Mở trang trắng để In)
        public ActionResult InBangDiem(string maLopTC)
        {
            if (string.IsNullOrEmpty(maLopTC)) return RedirectToAction("Index");
            ViewBag.MaLopTC = maLopTC;
            var danhSachDiem = _teacherDAO.GetDanhSachDiem(maLopTC);
            return View(danhSachDiem);
        }
    }
}
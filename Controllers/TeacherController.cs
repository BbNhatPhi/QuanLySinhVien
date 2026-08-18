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

        // POST: /Teacher/LuuDiemDanh (Xử lý khi bấm nút Lưu Điểm Danh)
        [HttpPost]
        public ActionResult LuuDiemDanh(string[] MaSV_Vang, string maLopTC)
        {
            if (MaSV_Vang != null)
            {
                foreach (var maSV in MaSV_Vang)
                {
                    _teacherDAO.TruDiemChuyenCan(maSV, maLopTC);
                }
                TempData["Success"] = "Đã lưu điểm danh! Các sinh viên vắng mặt đã bị trừ điểm chuyên cần.";
            }
            return RedirectToAction("DanhSachSinhVien", new { maLopTC = maLopTC });
        }

        // GET: /Teacher/NhapDiem (Hiển thị Form nhập điểm)
        public ActionResult NhapDiem(string maLopTC)
        {
            if (string.IsNullOrEmpty(maLopTC)) return RedirectToAction("Index");
            ViewBag.MaLopTC = maLopTC;
            var danhSachDiem = _teacherDAO.GetDanhSachDiem(maLopTC);
            return View(danhSachDiem);
        }

        // POST: /Teacher/LuuDiem (Xử lý khi bấm nút Lưu Bảng Điểm)
        [HttpPost]
        public ActionResult LuuDiem(string maLopTC, string[] MaSV, double[] DiemCC, double[] DiemGK, double[] DiemCK)
        {
            if (MaSV != null)
            {
                for (int i = 0; i < MaSV.Length; i++)
                {
                    _teacherDAO.UpdateDiem(MaSV[i], maLopTC, DiemCC[i], DiemGK[i], DiemCK[i]);
                }
                TempData["Success"] = "Đã lưu bảng điểm thành công! Điểm tổng kết đã được tự động tính toán.";
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
        // Đổi tên hàm thành ChonLopInDiem cho ngắn gọn và khớp
        public ActionResult ChonLopInDiem()
        {
            ViewBag.ChucNang = "InDiem";
            ViewBag.Title = "Chọn Lớp Để In Bảng Điểm";
            return View("XemLop", _teacherDAO.GetLichDay(User.Identity.Name));
        }
        [HttpPost]
        public ActionResult LuuDiemDanh(FormCollection form, string maLopHocPhan, string hocKy)
        {
            // Lấy danh sách sinh viên và trạng thái điểm danh từ giao diện gửi lên
            string[] maSVs = form.GetValues("maSV");
            string[] trangThais = form.GetValues("trangThaiVang");

            if (maSVs != null && trangThais != null)
            {
                for (int i = 0; i < maSVs.Length; i++)
                {
                    string maSV = maSVs[i];
                    string trangThai = trangThais[i]; // Có mặt, Có phép, Không phép

                    // ==========================================
                    // 1. Code cũ của bạn: Lưu lịch sử điểm danh vào CSDL
                    // ==========================================
                    // _teacherDAO.InsertDiemDanh(maLopHocPhan, maSV, trangThai, DateTime.Now);

                    // ==========================================
                    // 2. TÍNH NĂNG MỚI: TỰ ĐỘNG TRỪ ĐIỂM RÈN LUYỆN
                    // ==========================================
                    if (trangThai == "Có phép" || trangThai == "Không phép")
                    {
                        // Gọi hàm trừ điểm mà chúng ta vừa tạo ở DAO
                        _teacherDAO.TruDiemVangHoc(maSV, hocKy, trangThai);
                    }
                }

                TempData["SuccessMsg"] = "Đã lưu điểm danh và tự động cập nhật điểm rèn luyện thành công!";
            }
            else
            {
                TempData["ErrorMsg"] = "Không có dữ liệu điểm danh!";
            }

            // Trả về trang danh sách lớp sau khi lưu xong
            return RedirectToAction("ChonLopDiemDanh");
        }
    }
}
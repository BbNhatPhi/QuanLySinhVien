using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StudentManagementSystem.DAO;
using StudentManagementSystem.Models;
namespace StudentManagementSystem.Controllers
{
    // BẢO MẬT: Chỉ Sinh viên mới được phép truy cập
    [Authorize(Roles = "SinhVien")]
    public class StudentController : Controller
    {
        private StudentDAO _studentDAO = new StudentDAO();

        // GET: /Student/Index (Mặc định vào sẽ thấy Thời khóa biểu)
        public ActionResult Index()
        {
            string username = User.Identity.Name;
            var tkb = _studentDAO.GetTKB(username);
            return View(tkb);
        }

        // GET: /Student/XemDiem
        public ActionResult XemDiem()
        {
            string username = User.Identity.Name;

            // KIỂM TRA NGHIỆP VỤ: Đã đánh giá giảng viên hết chưa?
            int soMonChuaDanhGia = _studentDAO.DemSoMonChuaDanhGia(username);
            if (soMonChuaDanhGia > 0)
            {
                TempData["WarningMsg"] = $"Bạn còn {soMonChuaDanhGia} học phần chưa hoàn thành đánh giá giảng viên. Theo quy định đào tạo, bạn cần hoàn tất khảo sát để mở khóa bảng điểm!";
                return RedirectToAction("DanhGiaGiangVien");
            }

            var bangDiem = _studentDAO.GetBangDiem(username);
            return View(bangDiem);
        }

        // GET: /Student/DangKyHocPhan
        public ActionResult DangKyHocPhan()
        {
            var danhSachLop = _studentDAO.GetLopHocPhanMoDangKy();
            return View(danhSachLop);
        }

        // POST: Xử lý nút bấm Đăng ký
        [HttpPost]
        public ActionResult XuLyDangKy(string maLopHP)
        {
            // Lấy tên đăng nhập của sinh viên hiện tại (chính là Mã Sinh Viên)
            string username = User.Identity.Name;

            string result = _studentDAO.DangKyLop(username, maLopHP);

            if (result == "Success")
            {
                TempData["SuccessMsg"] = $"Chúc mừng! Đăng ký thành công lớp {maLopHP}.";
            }
            else
            {
                TempData["ErrorMsg"] = result; // Hiện thông báo (Trùng lớp, lớp đầy,...)
            }

            return RedirectToAction("DangKyHocPhan");
        }

       

        // ==========================================
        // THANH TOÁN HỌC PHÍ ONLINE
        // ==========================================

        // GET: /Student/ThanhToan (Hiển thị form thanh toán cho 1 môn)
        public ActionResult ThanhToan(string maLopHP, string tenMon, double soTien)
        {
            ViewBag.MaLopHP = maLopHP;
            ViewBag.TenMon = tenMon;
            ViewBag.SoTien = soTien;
            return View();
        }

       

        // ==========================================
        // THÔNG BÁO VÀ THÔNG TIN CÁ NHÂN
        // ==========================================

        // GET: /Student/XemThongBao
        public ActionResult XemThongBao()
        {
            var danhSachThongBao = _studentDAO.GetThongBao();
            return View(danhSachThongBao);
        }

        // GET: /Student/CapNhatThongTin
        public ActionResult CapNhatThongTin()
        {
            string username = User.Identity.Name;
            var sv = _studentDAO.GetThongTinCaNhan(username);
            if (sv == null)
            {
                return RedirectToAction("Index", "Home"); // Tránh lỗi nếu tài khoản bị lỗi
            }
            return View(sv);
        }

        // POST: Xử lý lưu thông tin
        [HttpPost]
        public ActionResult CapNhatThongTin(SinhVien sv)
        {
            string result = _studentDAO.UpdateThongTinCaNhan(sv);

            if (result == "Success")
            {
                TempData["SuccessMsg"] = "Lưu thay đổi thành công! Hồ sơ của bạn đã được cập nhật.";
            }
            else
            {
                TempData["ErrorMsg"] = "Đã xảy ra lỗi: " + result;
            }

            return RedirectToAction("CapNhatThongTin");
        }
        // GET: /Student/XemLichThi
        public ActionResult XemLichThi()
        {
            string username = User.Identity.Name;
            var lichThi = _studentDAO.GetLichThiSinhVien(username);
            return View(lichThi);
        }
        // POST: /Student/XacNhanThanhToan (Xử lý khi bấm nút Xác nhận)
        [HttpPost]
        public ActionResult XacNhanThanhToan(string maLopHP, double soTien)
        {
            // Lấy tài khoản sinh viên đang đăng nhập
            string username = User.Identity.Name;

            // Gọi DAO để cập nhật trạng thái môn này thành "Đã thanh toán"
            _studentDAO.ThanhToanHocPhi(username, maLopHP);

            TempData["SuccessMsg"] = $"Giao dịch thành công! Bạn đã nộp {soTien:N0} VNĐ cho học phần {maLopHP}.";
            return RedirectToAction("XemHocPhi");
        }
        // GET: /Student/XemHocPhi
        public ActionResult XemHocPhi()
        {
            string username = User.Identity.Name;

            // 1. Lấy danh sách nợ học phí (Truyền vào Model như cũ)
            var danhSachHocPhi = _studentDAO.GetHocPhi(username);

            // 2. Lấy danh sách lịch sử giao dịch (Truyền qua ViewBag)
            ViewBag.LichSuGiaoDich = _studentDAO.GetLichSuThanhToan(username);

            return View(danhSachHocPhi);
        }
        public ActionResult TienDoHocTap()
        {
            string username = User.Identity.Name;
            var tienDo = _studentDAO.GetTienDoHocTap(username);
            return View(tienDo);
        }
        // GET: /Student/DichVuHanhChinh
        public ActionResult DichVuHanhChinh()
        {
            string username = User.Identity.Name;
            var lichSu = _studentDAO.GetLichSuYeuCau(username);
            return View(lichSu);
        }

        // POST: /Student/GuiYeuCau
        [HttpPost]
        public ActionResult GuiYeuCau(string loaiDichVu, string lyDo)
        {
            string username = User.Identity.Name;
            string result = _studentDAO.GuiYeuCauHanhChinh(username, loaiDichVu, lyDo);

            if (result == "Success")
            {
                TempData["SuccessMsg"] = "Gửi yêu cầu thành công! Vui lòng theo dõi trạng thái xử lý.";
            }
            else
            {
                TempData["ErrorMsg"] = "Lỗi: " + result;
            }

            return RedirectToAction("DichVuHanhChinh");
        }
        // GET: /Student/DanhGiaGiangVien
        public ActionResult DanhGiaGiangVien()
        {
            string username = User.Identity.Name;
            var list = _studentDAO.GetDanhSachCanDanhGia(username);
            return View(list);
        }

        // POST: /Student/LuuDanhGia
        [HttpPost]
        public ActionResult LuuDanhGia(string maLopHP, int diemDanhGia, string nhanXet)
        {
            string username = User.Identity.Name;
            string result = _studentDAO.LuuDanhGia(username, maLopHP, diemDanhGia, nhanXet);

            if (result == "Success")
            {
                TempData["SuccessMsg"] = $"Đã lưu đánh giá cho học phần {maLopHP} thành công!";
            }
            else
            {
                TempData["ErrorMsg"] = "Lỗi: " + result;
            }

            return RedirectToAction("DanhGiaGiangVien");
        }
        // GET: /Student/XemDiemRenLuyen
        public ActionResult XemDiemRenLuyen()
        {
            // Lấy mã sinh viên từ tài khoản đang đăng nhập (VD: User.Identity.Name)
            string maSV = User.Identity.Name;

            // Khởi tạo DAO để lấy dữ liệu (điều chỉnh tên biến dao cho khớp với file của bạn)
            StudentManagementSystem.DAO.StudentDAO dao = new StudentManagementSystem.DAO.StudentDAO();
            var dsDiem = dao.GetDiemRenLuyenByMaSV(maSV);

            return View(dsDiem);
        }
    }
}
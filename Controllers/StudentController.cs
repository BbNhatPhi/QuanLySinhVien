using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StudentManagementSystem.Helpers;
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
            
            ViewBag.ThongTin = _studentDAO.GetThongTinCaNhan(username);
            ViewBag.TienDo = _studentDAO.GetTienDoHocTap(username);
            
            var tkb = _studentDAO.GetTKB(username);
            var lichThi = _studentDAO.GetLichThiSinhVien(username);
            var thongBao = _studentDAO.GetThongBao();

            ViewBag.SoLichHoc = tkb.Count;
            ViewBag.SoLichThi = lichThi.Count;
            ViewBag.SoThongBao = thongBao.Count;
            ViewBag.DanhSachMon = tkb; // Để hiển thị "Lớp học phần" ở góc dưới phải

            return View();
        }

        public ActionResult TKB()
        {
            string username = User.Identity.Name;
            var tkb = _studentDAO.GetTKB(username);
            return View("TKB", tkb);
        }

        // GET: /Student/XemDiem
        public ActionResult XemDiem()
        {
            string username = User.Identity.Name;

            // KIỂM TRA NGHIỆP VỤ: Đã đánh giá giảng viên hết chưa?
            

            var bangDiem = _studentDAO.GetBangDiem(username);
            return View(bangDiem);
        }

        // TÍNH NĂNG MỚI: In bảng điểm học tập tích lũy cá nhân
        public ActionResult InBangDiemCaNhan()
        {
            string username = User.Identity.Name;
            ViewBag.SinhVien = _studentDAO.GetThongTinCaNhan(username);
            var bangDiem = _studentDAO.GetBangDiem(username);
            return View(bangDiem);
        }

        // GET: /Student/DangKyHocPhan
        public ActionResult DangKyHocPhan()
        {
            string username = User.Identity.Name;

            // 1. Lấy danh sách các lớp đang mở
            var danhSachLop = _studentDAO.GetLopHocPhanMoDangKy(User.Identity.Name);

            // 2. BẮT BUỘC: Lấy các lớp đã đăng ký và truyền sang View để làm mờ nút
            ViewBag.ListMaLopDaDK = _studentDAO.GetMaLopDaDangKy(username);

            // 3. TÍNH NĂNG MỚI: Lấy chi tiết các lớp đã đăng ký để sinh viên tiện theo dõi & hủy
            ViewBag.ListLopDaDKChiTiet = _studentDAO.GetDanhSachLopDaDangKyChiTiet(username);

            return View(danhSachLop);
        }

        // POST: Xử lý nút bấm Đăng ký
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        // POST: Xử lý nút bấm Hủy đăng ký học phần (Rút môn)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HuyDangKy(string maLopHP)
        {
            string username = User.Identity.Name;

            string result = _studentDAO.HuyDangKyLop(username, maLopHP);

            if (result == "Success")
            {
                TempData["SuccessMsg"] = $"Đã hủy đăng ký thành công lớp {maLopHP}!";
            }
            else
            {
                TempData["ErrorMsg"] = result;
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
            
            // Nếu có nhiều môn (phân cách bằng dấu phẩy)
            if (maLopHP != null && maLopHP.Contains(","))
            {
                ViewBag.TenMon = "Thanh toán nhiều học phần";
            }
            else
            {
                ViewBag.TenMon = tenMon;
            }
            
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
        [ValidateAntiForgeryToken]
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
        [ValidateAntiForgeryToken]
        public ActionResult XacNhanThanhToan(string maLopHP, double soTien)
        {
            string username = User.Identity.Name;
            bool allSuccess = true;
            
            // Xử lý thanh toán nhiều môn cùng lúc
            string[] maLopList = maLopHP.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach(var ma in maLopList)
            {
                bool isSuccess = _studentDAO.ThanhToanHocPhi(username, ma.Trim());
                if (!isSuccess) allSuccess = false;
            }

            if (allSuccess)
            {
                StudentManagementSystem.Helpers.LogHelper.Log($"Sinh viên thanh toán thành công {soTien:N0} VNĐ cho {maLopList.Length} môn.");
                TempData["SuccessMsg"] = $"Giao dịch thành công! Bạn đã thanh toán {soTien:N0} VNĐ cho {maLopList.Length} học phần.";
            }
            else
            {
                TempData["ErrorMsg"] = "Có lỗi xảy ra trong quá trình thanh toán một số môn!";
            }

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
            ViewBag.ChuongTrinhKhung = _studentDAO.GetChuongTrinhKhung(username);
            ViewBag.ThongTin = _studentDAO.GetThongTinCaNhan(username);
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
        [ValidateAntiForgeryToken]
        public ActionResult GuiYeuCau(string loaiDichVu, string moTa, int soLuong = 1)
        {
            string result = _studentDAO.GuiYeuCauHanhChinh(User.Identity.Name, loaiDichVu, moTa, soLuong);
            if (result == "Success")
                TempData["SuccessMsg"] = "Đã gửi yêu cầu thành công!";
            else
                TempData["ErrorMsg"] = "Lỗi: " + result;

            return RedirectToAction("DichVuHanhChinh");
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
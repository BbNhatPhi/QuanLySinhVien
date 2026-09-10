using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StudentManagementSystem.DAO;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Controllers
{
    // BẢO MẬT: Chỉ Nhân viên Phòng đào tạo mới được vào
    [Authorize(Roles = "NhanVien")]
    public class StaffController : Controller
    {
        private StaffDAO _staffDAO = new StaffDAO();

        // ==========================================
        // QUẢN LÝ SINH VIÊN
        // ==========================================

        public ActionResult Index()
        {
            return RedirectToAction("QuanLySinhVien");
        }

        public ActionResult QuanLySinhVien()
        {
            var sinhViens = _staffDAO.GetAllSinhVien();

            // Chỉ nạp danh sách Khoa (Đã loại bỏ hoàn toàn Ngành)
            ViewBag.ListKhoa = _staffDAO.GetAllKhoa();

            return View(sinhViens);
        }

        [HttpPost]
        public ActionResult UpdateStatus(string maSV, string trangThai)
        {
            if (!string.IsNullOrEmpty(maSV) && !string.IsNullOrEmpty(trangThai))
            {
                _staffDAO.UpdateTrangThaiHocTap(maSV, trangThai);
                TempData["Success"] = $"Đã cập nhật trạng thái cho sinh viên {maSV} thành: {trangThai}";
            }
            return RedirectToAction("QuanLySinhVien");
        }

        [HttpPost]
        public ActionResult ThemSinhVien(SinhVien sv)
        {
            string result = _staffDAO.AddSinhVien(sv);
            if (result == "Success")
            {
                TempData["Success"] = "Thêm sinh viên mới thành công!";
            }
            else
            {
                TempData["Error"] = "Lỗi SQL: " + result;
            }
            return RedirectToAction("QuanLySinhVien");
        }

        public ActionResult XoaSinhVien(string maSV)
        {
            if (_staffDAO.DeleteSinhVien(maSV))
            {
                TempData["Success"] = $"Đã xóa thành công sinh viên {maSV}!";
            }
            return RedirectToAction("QuanLySinhVien");
        }

        // ==========================================
        // QUẢN LÝ MÔN HỌC
        // ==========================================

        public ActionResult ManageMonHoc()
        {
            var monHocs = _staffDAO.GetAllMonHoc();
            return View(monHocs);
        }

        [HttpPost]
        public ActionResult CreateMonHoc(string maMon, string tenMon, int soTinChi)
        {
            bool isSuccess = _staffDAO.AddMonHoc(maMon, tenMon, soTinChi);

            if (isSuccess)
            {
                TempData["Success"] = "Đã thêm môn học mới thành công!";
            }
            else
            {
                TempData["Error"] = "Thêm thất bại! Mã môn học này có thể đã tồn tại.";
            }

            return RedirectToAction("ManageMonHoc");
        }

        [HttpPost]
        public ActionResult EditMonHoc(string maMon, string tenMon, int soTinChi)
        {
            bool isSuccess = _staffDAO.UpdateMonHoc(maMon, tenMon, soTinChi);

            if (isSuccess)
            {
                TempData["Success"] = "Đã cập nhật thông tin môn học thành công!";
            }
            else
            {
                TempData["Error"] = "Có lỗi xảy ra khi cập nhật môn học!";
            }

            return RedirectToAction("ManageMonHoc");
        }

        // ==========================================
        // QUẢN LÝ LỚP HỌC PHẦN (MỞ LỚP)
        // ==========================================

        public ActionResult LopHocPhanList()
        {
            ViewBag.ListMonHoc = _staffDAO.GetAllMonHoc();
            ViewBag.ListGiangVien = _staffDAO.GetAllGiangVien();

            return View(_staffDAO.GetAllLopHocPhan());
        }

        [HttpPost]
        public ActionResult AddLopHocPhan(LopHocPhan lhp)
        {
            string result = _staffDAO.AddLopHocPhan(lhp);

            if (result == "Success")
            {
                TempData["SuccessMsg"] = "Mở lớp học phần thành công! Sẵn sàng cho sinh viên đăng ký.";
            }
            else
            {
                TempData["ErrorMsg"] = "Lỗi: " + result;
            }
            return RedirectToAction("LopHocPhanList");
        }

        // ==========================================
        // QUẢN LÝ THỜI KHÓA BIỂU
        // ==========================================

        public ActionResult TKBList()
        {
            ViewBag.ListLopHocPhan = _staffDAO.GetAllLopHocPhan();
            return View(_staffDAO.GetAllTKB());
        }

        [HttpPost]
        public ActionResult AddTKB(ThoiKhoaBieu tkb)
        {
            string result = _staffDAO.AddTKB(tkb);

            if (result == "Success")
            {
                TempData["SuccessMsg"] = "Xếp lịch Thời khóa biểu thành công!";
            }
            else
            {
                TempData["ErrorMsg"] = "Lỗi: " + result;
            }
            return RedirectToAction("TKBList");
        }

        // ==========================================
        // QUẢN LÝ GIẢNG VIÊN
        // ==========================================

        public ActionResult GiangVienList()
        {
            ViewBag.ListKhoa = _staffDAO.GetAllKhoa();
            ViewBag.ListMonHoc = _staffDAO.GetAllMonHoc();

            return View(_staffDAO.GetDanhSachGiangVien());
        }

        [HttpPost]
        public ActionResult AddGiangVien(GiangVien gv)
        {
            string result = _staffDAO.AddGiangVien(gv);

            if (result == "Success")
            {
                TempData["SuccessMsg"] = $"Thêm giảng viên {gv.HoTen} thành công. Tài khoản đăng nhập đã được tự động tạo!";
            }
            else
            {
                TempData["ErrorMsg"] = "Lỗi: " + result;
            }
            return RedirectToAction("GiangVienList");
        }

        public ActionResult DeleteGiangVien(string maGV)
        {
            if (_staffDAO.DeleteGiangVien(maGV))
            {
                TempData["SuccessMsg"] = $"Đã xóa thành công giảng viên {maGV}!";
            }
            else
            {
                TempData["ErrorMsg"] = "Xóa thất bại! Giảng viên này có thể đang có dữ liệu lớp học ràng buộc.";
            }
            return RedirectToAction("GiangVienList");
        }

        // ==========================================
        // QUẢN LÝ LỊCH THI
        // ==========================================

        public ActionResult ManageLichThi()
        {
            ViewBag.DsLopHP = _staffDAO.GetAllLopHocPhan();
            var lichThi = _staffDAO.GetAllLichThi();
            return View(lichThi);
        }

        [HttpPost]
        public ActionResult AddLichThi(string maLopHP, DateTime ngayThi, int caThi, string phongThi)
        {
            string result = _staffDAO.AddLichThi(maLopHP, ngayThi, caThi, phongThi);
            if (result == "Success") TempData["SuccessMsg"] = "Đã lên lịch thi thành công!";
            else TempData["ErrorMsg"] = result;

            return RedirectToAction("ManageLichThi");
        }

        [HttpGet]
        public JsonResult GetPhongTrong(DateTime ngayThi, int caThi)
        {
            var phongTrong = _staffDAO.GetPhongTrongLichThi(ngayThi, caThi);
            return Json(phongTrong, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult EditLichThi(string maLopHP, DateTime ngayThi, int caThi, string phongThi)
        {
            string result = _staffDAO.UpdateLichThi(maLopHP, ngayThi, caThi, phongThi);
            if (result == "Success") TempData["SuccessMsg"] = "Đã cập nhật lịch thi thành công!";
            else TempData["ErrorMsg"] = result;

            return RedirectToAction("ManageLichThi");
        }

        [HttpPost]
        public ActionResult DeleteLichThi(string maLopHP)
        {
            if (_staffDAO.DeleteLichThi(maLopHP)) TempData["SuccessMsg"] = $"Đã hủy lịch thi của lớp {maLopHP} thành công!";
            else TempData["ErrorMsg"] = "Lỗi: Không thể xóa lịch thi này do dữ liệu đang được ràng buộc!";

            return RedirectToAction("ManageLichThi");
        }

        // ==========================================
        // QUẢN LÝ THÔNG BÁO
        // ==========================================

        public ActionResult ManageThongBao()
        {
            var dsThongBao = _staffDAO.GetAllThongBao();
            return View(dsThongBao);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddThongBao(string tieuDe, string noiDung, string doiTuong)
        {
            string username = User.Identity.Name;
            bool isSuccess = _staffDAO.AddThongBao(tieuDe, noiDung, username, doiTuong);

            if (isSuccess)
            {
                TempData["SuccessMsg"] = "Đăng bản tin thông báo mới thành công!";
            }
            else
            {
                TempData["ErrorMsg"] = "Có lỗi xảy ra trong quá trình đăng tin!";
            }

            return RedirectToAction("ManageThongBao");
        }

        // ==========================================
        // QUẢN LÝ ĐIỂM RÈN LUYỆN
        // ==========================================

        public ActionResult ManageDiemRenLuyen()
        {
            var dsDiem = _staffDAO.GetAllDiemRenLuyen();
            return View(dsDiem);
        }

        private string TinhXepLoai(int diem)
        {
            if (diem >= 90) return "Xuất sắc";
            if (diem >= 80) return "Tốt";
            if (diem >= 65) return "Khá";
            if (diem >= 50) return "Trung bình";
            return "Yếu";
        }

        [HttpPost]
        public ActionResult AddDiemRenLuyen(string maSV, string hocKy, int diem)
        {
            string xepLoai = TinhXepLoai(diem);
            if (_staffDAO.AddDiemRenLuyen(maSV, hocKy, diem, xepLoai))
                TempData["SuccessMsg"] = $"Đã nhập điểm rèn luyện cho SV {maSV} thành công!";
            else
                TempData["ErrorMsg"] = $"Lỗi: Sinh viên {maSV} đã có điểm rèn luyện trong {hocKy}!";

            return RedirectToAction("ManageDiemRenLuyen");
        }

        [HttpPost]
        public ActionResult EditDiemRenLuyen(int id, int diem)
        {
            string xepLoai = TinhXepLoai(diem);
            if (_staffDAO.UpdateDiemRenLuyen(id, diem, xepLoai))
                TempData["SuccessMsg"] = "Cập nhật điểm rèn luyện thành công!";
            else
                TempData["ErrorMsg"] = "Có lỗi xảy ra khi cập nhật điểm!";

            return RedirectToAction("ManageDiemRenLuyen");
        }

        [HttpPost]
        public ActionResult DeleteDiemRenLuyen(int id)
        {
            if (_staffDAO.DeleteDiemRenLuyen(id))
                TempData["SuccessMsg"] = "Đã xóa điểm rèn luyện thành công!";
            else
                TempData["ErrorMsg"] = "Lỗi khi xóa dữ liệu!";

            return RedirectToAction("ManageDiemRenLuyen");
        }

        // ==========================================
        // DỊCH VỤ HÀNH CHÍNH (XỬ LÝ YÊU CẦU)
        // ==========================================

        public ActionResult QuanLyYeuCau()
        {
            var list = _staffDAO.GetAllYeuCau();
            return View(list);
        }

        [HttpPost]
        public ActionResult CapNhatTrangThaiYeuCau(int MaYC, string TrangThaiMoi)
        {
            bool check = _staffDAO.UpdateTrangThaiYeuCau(MaYC, TrangThaiMoi);
            if (check)
            {
                TempData["SuccessMsg"] = $"Đã cập nhật yêu cầu #{MaYC} thành: {TrangThaiMoi}";
            }
            else
            {
                TempData["ErrorMsg"] = "Có lỗi xảy ra, vui lòng thử lại!";
            }
            return RedirectToAction("QuanLyYeuCau");
        }
    }
}
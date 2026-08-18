using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StudentManagementSystem.DAO;
using StudentManagementSystem.Models;
namespace StudentManagementSystem.Controllers
{
    // BẢO MẬT: Chỉ Admin mới được vào các trang này
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private AdminDAO _adminDAO = new AdminDAO();

        // GET: /Admin/Index (Dashboard)
        public ActionResult Index()
        {
            var stats = _adminDAO.GetDashboardStats();
            return View(stats);
        }

        // ==========================================
        // QUẢN LÝ TÀI KHOẢN
        // ==========================================

        // GET: /Admin/ManageUsers
        public ActionResult ManageUsers()
        {
            var users = _adminDAO.GetAllUsers();
            return View(users);
        }

        // POST: /Admin/CreateUser
        [HttpPost]
        public ActionResult CreateUser(string username, string password, int roleId, string hoTen)
        {
            try
            {
                _adminDAO.CreateUser(username, password, roleId, hoTen);
                TempData["SuccessMsg"] = "Tạo tài khoản thành công!";
            }
            catch
            {
                TempData["ErrorMsg"] = "Tên đăng nhập đã tồn tại hoặc có lỗi xảy ra!";
            }
            return RedirectToAction("ManageUsers");
        }

        // POST: /Admin/ToggleStatus
        [HttpPost]
        public ActionResult ToggleStatus(int userId, bool currentStatus)
        {
            _adminDAO.ToggleUserStatus(userId, !currentStatus);
            TempData["SuccessMsg"] = !currentStatus ? "Đã mở khóa tài khoản!" : "Đã khóa tài khoản!";
            return RedirectToAction("ManageUsers");
        }

        // ==========================================
        // QUẢN LÝ KHOA
        // ==========================================

        // GET: /Admin/ManageKhoa
        public ActionResult ManageKhoa()
        {
            var khoas = _adminDAO.GetAllKhoa();
            return View(khoas);
        }

        // POST: /Admin/CreateKhoa
        [HttpPost]
        public ActionResult CreateKhoa(string tenKhoa, string lienHe)
        {
            _adminDAO.AddKhoa(tenKhoa, lienHe);
            TempData["SuccessMsg"] = "Đã thêm khoa mới thành công!";
            return RedirectToAction("ManageKhoa");
        }

        // GET: /Admin/DeleteKhoa
        public ActionResult DeleteKhoa(int id)
        {
            // Lưu ý: Nếu AdminDAO của bạn chưa có hàm DeleteKhoa, hãy bổ sung nó vào DAO nhé
            try
            {
                // _adminDAO.DeleteKhoa(id); // Bỏ comment dòng này nếu bạn đã viết hàm DeleteKhoa trong AdminDAO
                TempData["SuccessMsg"] = "Xóa khoa thành công!";
            }
            catch
            {
                TempData["ErrorMsg"] = "Không thể xóa khoa này vì đang có dữ liệu giảng viên hoặc ngành học trực thuộc!";
            }
            return RedirectToAction("ManageKhoa");
        }

        // ==========================================
        // QUẢN LÝ NGÀNH
        // ==========================================

        // GET: /Admin/ManageNganh
        public ActionResult ManageNganh()
        {
            var nganhs = _adminDAO.GetAllNganh();

            // Lấy danh sách Khoa nạp vào ViewBag để dùng cho thẻ <select> ở View
            ViewBag.ListKhoa = _adminDAO.GetAllKhoa();

            return View(nganhs);
        }

        // POST: /Admin/CreateNganh
        [HttpPost]
        public ActionResult CreateNganh(string tenNganh, int khoaId)
        {
            _adminDAO.AddNganh(tenNganh, khoaId);
            TempData["SuccessMsg"] = "Đã thêm ngành học mới thành công!";
            return RedirectToAction("ManageNganh");
        }
        // GET: /Admin/DeleteUser
        public ActionResult DeleteUser(int id)
        {
            if (_adminDAO.DeleteUser(id))
            {
                TempData["SuccessMsg"] = "Đã xóa tài khoản thành công!";
            }
            else
            {
                TempData["ErrorMsg"] = "Không thể xóa! Tài khoản này đang chứa dữ liệu hồ sơ (Sinh Viên/Giảng Viên). Hãy dùng tính năng Khóa tài khoản thay thế.";
            }
            return RedirectToAction("ManageUsers");
        }
        // POST: /Admin/ChangeUserRole (Đổi quyền)
        [HttpPost]
        public ActionResult ChangeUserRole(int userId, int newRoleId)
        {
            _adminDAO.ChangeUserRole(userId, newRoleId);
            TempData["SuccessMsg"] = "Đã cập nhật phân quyền mới cho tài khoản thành công!";
            return RedirectToAction("ManageUsers");
        }
        // ==========================================
        // BACKUP DỮ LIỆU
        // ==========================================

        // GET: /Admin/BackupData
        public ActionResult BackupData()
        {
            return View();
        }

        // POST: /Admin/ExecuteBackup
        [HttpPost]
        public ActionResult ExecuteBackup()
        {
            string result = _adminDAO.BackupDatabase();

            if (result.StartsWith("SUCCESS|"))
            {
                string filePath = result.Split('|')[1];
                TempData["SuccessMsg"] = "Sao lưu dữ liệu thành công! File được lưu an toàn tại: " + filePath;
            }
            else
            {
                TempData["ErrorMsg"] = "Sao lưu thất bại! Hãy chắc chắn bạn đã tạo thư mục C:\\Backup_SMS. Lỗi chi tiết: " + result;
            }

            return RedirectToAction("BackupData");
        }
        [HttpPost]
        public ActionResult EditNganh(int nganhId, string tenNganh, int khoaId)
        {
            bool result = _adminDAO.UpdateNganh(nganhId, tenNganh, khoaId);

            if (result)
            {
                TempData["SuccessMsg"] = "Đã cập nhật thông tin Ngành thành công!";
            }
            else
            {
                TempData["ErrorMsg"] = "Có lỗi xảy ra khi cập nhật Ngành!";
            }

            // Lưu ý: Đổi "NganhList" thành tên Action hiển thị danh sách Ngành hiện tại của bạn (vd: ManageNganh, DanhSachNganh...)
            return RedirectToAction("NganhList");
        }
        // ==========================================
        // QUẢN LÝ PHÒNG HỌC
        // ==========================================

        public ActionResult ManagePhongHoc()
        {
            var dsPhong = _adminDAO.GetAllPhongHoc();
            return View(dsPhong);
        }

        [HttpPost]
        public ActionResult AddPhongHoc(string maPhong, int sucChua, string loaiPhong, string tinhTrang)
        {
            if (_adminDAO.AddPhongHoc(maPhong, sucChua, loaiPhong, tinhTrang))
                TempData["SuccessMsg"] = $"Thêm phòng {maPhong} thành công!";
            else
                TempData["ErrorMsg"] = $"Lỗi: Mã phòng {maPhong} đã tồn tại hoặc hệ thống gặp sự cố!";

            return RedirectToAction("ManagePhongHoc");
        }

        [HttpPost]
        public ActionResult EditPhongHoc(string maPhong, int sucChua, string loaiPhong, string tinhTrang)
        {
            if (_adminDAO.UpdatePhongHoc(maPhong, sucChua, loaiPhong, tinhTrang))
                TempData["SuccessMsg"] = "Cập nhật thông tin phòng học thành công!";
            else
                TempData["ErrorMsg"] = "Có lỗi xảy ra khi cập nhật phòng học!";

            return RedirectToAction("ManagePhongHoc");
        }

        [HttpPost]
        public ActionResult DeletePhongHoc(string maPhong)
        {
            if (_adminDAO.DeletePhongHoc(maPhong))
                TempData["SuccessMsg"] = $"Đã xóa phòng {maPhong} khỏi hệ thống!";
            else
                TempData["ErrorMsg"] = "Không thể xóa phòng học này vì đang có dữ liệu xếp lịch ràng buộc!";

            return RedirectToAction("ManagePhongHoc");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudentManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // Nếu chưa đăng nhập, đá về Login
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            // Nếu đã đăng nhập, đá về đúng trang theo Role
            if (User.IsInRole("Admin")) return RedirectToAction("Index", "Admin");
            if (User.IsInRole("GiangVien")) return RedirectToAction("Index", "Teacher");
            if (User.IsInRole("NhanVien")) return RedirectToAction("Index", "Staff");
            if (User.IsInRole("SinhVien")) return RedirectToAction("Index", "Student");

            return View();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StudentManagementSystem.DAO;
using StudentManagementSystem.Utils;

namespace StudentManagementSystem.Controllers
{
    /// <summary>
    /// Controller xử lý thanh toán học phí qua cổng VNPAY Sandbox
    /// </summary>
    [Authorize(Roles = "SinhVien")]
    public class PaymentController : Controller
    {
        private StudentDAO _studentDAO = new StudentDAO();

        // =====================================================================
        // GET: /Payment/VNPay?maLopHP=xxx&soTien=xxx&tenMon=xxx
        // Tạo URL thanh toán và redirect sang VNPAY
        // =====================================================================
        [HttpGet]
        public ActionResult VNPay(string maLopHP, double soTien, string tenMon)
        {
            if (string.IsNullOrWhiteSpace(maLopHP) || soTien <= 0)
            {
                TempData["ErrorMsg"] = "Thông tin thanh toán không hợp lệ!";
                return RedirectToAction("XemHocPhi", "Student");
            }

            string orderInfo = $"NTTU {maLopHP} - {tenMon}";
            string ipAddress = VNPayHelper.GetClientIp(Request);
            long amount = (long)Math.Round(soTien);

            try
            {
                string paymentUrl = VNPayHelper.CreatePaymentUrl(maLopHP, amount, orderInfo, ipAddress);

                // Lưu thông tin vào Session để xác nhận khi callback
                Session["VNPay_MaLopHP"] = maLopHP;
                Session["VNPay_SoTien"] = soTien;
                Session["VNPay_TenMon"] = tenMon;

                // Redirect sang VNPAY Sandbox
                return Redirect(paymentUrl);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[PaymentController] Lỗi tạo URL VNPAY: " + ex.Message);
                TempData["ErrorMsg"] = "Không thể kết nối đến cổng thanh toán. Vui lòng thử lại!";
                return RedirectToAction("XemHocPhi", "Student");
            }
        }

        // =====================================================================
        // GET: /Payment/VNPayReturn
        // VNPAY gọi về đây sau khi sinh viên hoàn thành / hủy giao dịch
        // =====================================================================
        [HttpGet]
        [AllowAnonymous] // VNPAY callback không có session auth
        public ActionResult VNPayReturn()
        {
            // Lấy tất cả query params từ VNPAY
            var queryParams = Request.QueryString.AllKeys
                .Select(k => new KeyValuePair<string, string>(k, Request.QueryString[k]))
                .ToList();

            // Xác thực chữ ký để chống giả mạo
            bool isValidSignature = VNPayHelper.ValidateSignature(queryParams);

            if (!isValidSignature)
            {
                ViewBag.PaymentStatus = "INVALID";
                ViewBag.Message = "Chữ ký xác thực không hợp lệ! Giao dịch có thể bị giả mạo.";
                return View("VNPayResult");
            }

            string responseCode = Request.QueryString["vnp_ResponseCode"];
            string txnRef = Request.QueryString["vnp_TxnRef"] ?? "";
            string orderInfo = Request.QueryString["vnp_OrderInfo"] ?? "";
            string amount = Request.QueryString["vnp_Amount"] ?? "0";
            string transactionNo = Request.QueryString["vnp_TransactionNo"] ?? "";

            // Lấy MaLopHP từ txnRef (format: MALOPHI_yyyyMMddHHmmss)
            string maLopHP = txnRef.Contains("_") ? txnRef.Split('_')[0] : txnRef;
            double soTien = Math.Round(double.Parse(amount) / 100.0);
            string tenMon = Session["VNPay_TenMon"]?.ToString() ?? orderInfo;

            ViewBag.MaLopHP = maLopHP;
            ViewBag.SoTien = soTien;
            ViewBag.TenMon = tenMon;
            ViewBag.TransactionNo = transactionNo;
            ViewBag.OrderInfo = orderInfo;

            if (responseCode == "00")
            {
                // Giao dịch thành công — cập nhật DB
                if (User.Identity.IsAuthenticated)
                {
                    string username = User.Identity.Name;
                    bool updated = _studentDAO.ThanhToanHocPhi(username, maLopHP);
                    ViewBag.DbUpdated = updated;
                }
                else
                {
                    // Session có thể đã hết — thử lấy từ txnRef (chấp nhận rủi ro nhỏ trong sandbox)
                    ViewBag.DbUpdated = false;
                }

                ViewBag.PaymentStatus = "SUCCESS";
                ViewBag.Message = $"Thanh toán thành công! Học phần <strong>{maLopHP}</strong> đã được cập nhật.";
            }
            else if (responseCode == "24")
            {
                ViewBag.PaymentStatus = "CANCELLED";
                ViewBag.Message = "Bạn đã hủy giao dịch thanh toán.";
            }
            else
            {
                ViewBag.PaymentStatus = "FAILED";
                ViewBag.Message = $"Thanh toán thất bại! Mã lỗi VNPAY: {responseCode}.";
            }

            // Xóa session payment
            Session.Remove("VNPay_MaLopHP");
            Session.Remove("VNPay_SoTien");
            Session.Remove("VNPay_TenMon");

            return View("VNPayResult");
        }
    }
}

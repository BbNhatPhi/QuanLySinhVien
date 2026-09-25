using System;
using System.Net;
using System.Net.Mail;
using System.Configuration;
using System.Threading.Tasks;

namespace StudentManagementSystem.Utils
{
    /// <summary>
    /// Helper class để gửi email thông báo tự động qua SMTP
    /// Cấu hình SMTP trong Web.config -> appSettings
    /// </summary>
    public static class EmailHelper
    {
        private static readonly string SmtpHost = ConfigurationManager.AppSettings["SmtpHost"] ?? "smtp.gmail.com";
        private static readonly int SmtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"] ?? "587");
        private static readonly string SmtpUser = ConfigurationManager.AppSettings["SmtpUser"] ?? "";
        private static readonly string SmtpPass = ConfigurationManager.AppSettings["SmtpPass"] ?? "";
        private static readonly string SmtpFrom = ConfigurationManager.AppSettings["SmtpFrom"] ?? "noreply@university.edu.vn";
        private static readonly string SmtpFromName = ConfigurationManager.AppSettings["SmtpFromName"] ?? "Hệ thống Quản lý Sinh viên";
        private static readonly bool EmailEnabled = bool.Parse(ConfigurationManager.AppSettings["EmailEnabled"] ?? "false");

        /// <summary>
        /// Gửi email đồng bộ. Trả về true nếu thành công.
        /// </summary>
        public static bool SendEmail(string toEmail, string subject, string htmlBody)
        {
            if (!EmailEnabled || string.IsNullOrWhiteSpace(toEmail))
                return false;

            try
            {
                using (var client = new SmtpClient(SmtpHost, SmtpPort))
                {
                    client.EnableSsl = true;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(SmtpUser, SmtpPass);
                    client.Timeout = 10000; // 10 giây

                    var message = new MailMessage();
                    message.From = new MailAddress(SmtpFrom, SmtpFromName);
                    message.To.Add(new MailAddress(toEmail));
                    message.Subject = subject;
                    message.Body = htmlBody;
                    message.IsBodyHtml = true;

                    client.Send(message);
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log lỗi nhưng không ném exception ra ngoài (không làm gián đoạn luồng chính)
                System.Diagnostics.Debug.WriteLine($"[EmailHelper] Lỗi gửi email tới {toEmail}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gửi email không đồng bộ (fire and forget) — không chặn request
        /// </summary>
        public static void SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            if (!EmailEnabled || string.IsNullOrWhiteSpace(toEmail)) return;
            Task.Run(() => SendEmail(toEmail, subject, htmlBody));
        }

        // =====================================================================
        // CÁC TEMPLATE EMAIL DỰNG SẴN
        // =====================================================================

        /// <summary>
        /// Template: Thông báo duyệt yêu cầu hành chính
        /// </summary>
        public static void GuiEmailDuyetYeuCau(string toEmail, string hoTenSV, string loaiYeuCau, string trangThaiMoi, int maYC)
        {
            string subject = $"[QLSV] Cập nhật yêu cầu #{maYC} - {loaiYeuCau}";
            string color = trangThaiMoi == "Đã duyệt" ? "#28a745" : (trangThaiMoi == "Từ chối" ? "#dc3545" : "#ffc107");
            string icon = trangThaiMoi == "Đã duyệt" ? "✅" : (trangThaiMoi == "Từ chối" ? "❌" : "⏳");

            string body = $@"
<!DOCTYPE html>
<html>
<head><meta charset='UTF-8'></head>
<body style='font-family: Arial, sans-serif; background:#f4f4f4; padding:20px;'>
    <div style='max-width:600px; margin:0 auto; background:#fff; border-radius:8px; overflow:hidden; box-shadow:0 2px 8px rgba(0,0,0,0.1);'>
        <div style='background:#003366; padding:20px; text-align:center;'>
            <h2 style='color:#fff; margin:0;'>🎓 Hệ thống Quản lý Sinh viên</h2>
            <p style='color:#cce0ff; margin:5px 0 0;'>Thông báo tự động</p>
        </div>
        <div style='padding:30px;'>
            <p>Xin chào <strong>{hoTenSV}</strong>,</p>
            <p>Yêu cầu dịch vụ hành chính của bạn đã được cập nhật:</p>
            <div style='background:#f8f9fa; border-left:4px solid {color}; padding:15px; margin:20px 0; border-radius:4px;'>
                <p style='margin:0;'><strong>Mã yêu cầu:</strong> #{maYC}</p>
                <p style='margin:5px 0 0;'><strong>Loại dịch vụ:</strong> {loaiYeuCau}</p>
                <p style='margin:5px 0 0;'><strong>Trạng thái:</strong> 
                    <span style='color:{color}; font-weight:bold;'>{icon} {trangThaiMoi}</span>
                </p>
            </div>
            <p>Vui lòng đăng nhập vào cổng sinh viên để xem chi tiết.</p>
            <p style='color:#6c757d; font-size:13px;'>Email này được gửi tự động, vui lòng không trả lời.</p>
        </div>
        <div style='background:#f8f9fa; padding:15px; text-align:center; color:#6c757d; font-size:12px;'>
            © Phòng Đào tạo | Hệ thống Quản lý Sinh viên
        </div>
    </div>
</body>
</html>";
            SendEmailAsync(toEmail, subject, body);
        }

        /// <summary>
        /// Template: Thông báo giảng viên đã nhập xong điểm
        /// </summary>
        public static void GuiEmailThongBaoDiem(string toEmail, string hoTenSV, string tenMon, string maLopHP, string hocKy)
        {
            string subject = $"[QLSV] Điểm học phần {tenMon} đã được công bố";
            string body = $@"
<!DOCTYPE html>
<html>
<head><meta charset='UTF-8'></head>
<body style='font-family: Arial, sans-serif; background:#f4f4f4; padding:20px;'>
    <div style='max-width:600px; margin:0 auto; background:#fff; border-radius:8px; overflow:hidden; box-shadow:0 2px 8px rgba(0,0,0,0.1);'>
        <div style='background:#003366; padding:20px; text-align:center;'>
            <h2 style='color:#fff; margin:0;'>🎓 Hệ thống Quản lý Sinh viên</h2>
            <p style='color:#cce0ff; margin:5px 0 0;'>Thông báo điểm học phần</p>
        </div>
        <div style='padding:30px;'>
            <p>Xin chào <strong>{hoTenSV}</strong>,</p>
            <p>Điểm học phần của bạn đã được giảng viên công bố:</p>
            <div style='background:#e8f5e9; border-left:4px solid #28a745; padding:15px; margin:20px 0; border-radius:4px;'>
                <p style='margin:0;'><strong>📚 Học phần:</strong> {tenMon}</p>
                <p style='margin:5px 0 0;'><strong>Mã lớp:</strong> {maLopHP}</p>
                <p style='margin:5px 0 0;'><strong>Học kỳ:</strong> {hocKy}</p>
            </div>
            <p>Vui lòng đăng nhập vào <strong>cổng sinh viên → Xem Điểm</strong> để tra cứu điểm của bạn.</p>
            <p style='color:#6c757d; font-size:13px;'>Email này được gửi tự động, vui lòng không trả lời.</p>
        </div>
        <div style='background:#f8f9fa; padding:15px; text-align:center; color:#6c757d; font-size:12px;'>
            © Phòng Đào tạo | Hệ thống Quản lý Sinh viên
        </div>
    </div>
</body>
</html>";
            SendEmailAsync(toEmail, subject, body);
        }

        /// <summary>
        /// Template: Thông báo mới từ nhà trường gửi đến sinh viên
        /// </summary>
        public static void GuiEmailThongBaoMoi(string toEmail, string hoTenSV, string tieuDe, string tomTat)
        {
            string subject = $"[QLSV] Thông báo mới: {tieuDe}";
            string body = $@"
<!DOCTYPE html>
<html>
<head><meta charset='UTF-8'></head>
<body style='font-family: Arial, sans-serif; background:#f4f4f4; padding:20px;'>
    <div style='max-width:600px; margin:0 auto; background:#fff; border-radius:8px; overflow:hidden; box-shadow:0 2px 8px rgba(0,0,0,0.1);'>
        <div style='background:#003366; padding:20px; text-align:center;'>
            <h2 style='color:#fff; margin:0;'>🎓 Hệ thống Quản lý Sinh viên</h2>
            <p style='color:#cce0ff; margin:5px 0 0;'>Thông báo từ Nhà trường</p>
        </div>
        <div style='padding:30px;'>
            <p>Xin chào <strong>{hoTenSV}</strong>,</p>
            <p>Bạn có một thông báo mới từ Phòng Đào tạo:</p>
            <div style='background:#e3f2fd; border-left:4px solid #1565c0; padding:15px; margin:20px 0; border-radius:4px;'>
                <h4 style='margin:0 0 8px; color:#1565c0;'>📢 {tieuDe}</h4>
                <p style='margin:0; color:#333;'>{tomTat}</p>
            </div>
            <p>Vui lòng đăng nhập vào cổng sinh viên để xem nội dung đầy đủ.</p>
            <p style='color:#6c757d; font-size:13px;'>Email này được gửi tự động, vui lòng không trả lời.</p>
        </div>
        <div style='background:#f8f9fa; padding:15px; text-align:center; color:#6c757d; font-size:12px;'>
            © Phòng Đào tạo | Hệ thống Quản lý Sinh viên
        </div>
    </div>
</body>
</html>";
            SendEmailAsync(toEmail, subject, body);
        }
    }
}

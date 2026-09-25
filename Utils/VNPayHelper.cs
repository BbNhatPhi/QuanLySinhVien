using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Configuration;
using System.Linq;

namespace StudentManagementSystem.Utils
{
    /// <summary>
    /// VNPAY Sandbox Helper — Tích hợp cổng thanh toán VNPAY
    /// Tài liệu: https://sandbox.vnpayment.vn/apis/docs/
    /// Môi trường: Sandbox (test). Thay TmnCode + HashSecret bằng thông tin thật khi production.
    /// </summary>
    public static class VNPayHelper
    {
        // =====================================================================
        // CẤU HÌNH VNPAY SANDBOX
        // =====================================================================
        private static readonly string TmnCode = ConfigurationManager.AppSettings["VNPay_TmnCode"] ?? "DEMOV210";
        private static readonly string HashSecret = ConfigurationManager.AppSettings["VNPay_HashSecret"] ?? "RAOEXHYVSDDIIENYWSLDIIZTANXUXZTU";
        private static readonly string PaymentUrl = ConfigurationManager.AppSettings["VNPay_PaymentUrl"] ?? "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        private static readonly string ReturnUrl = ConfigurationManager.AppSettings["VNPay_ReturnUrl"] ?? "https://localhost:44311/Payment/VNPayReturn";
        private static readonly string Version = "2.1.0";
        private static readonly string Command = "pay";
        private static readonly string CurrCode = "VND";
        private static readonly string Locale = "vn";

        /// <summary>
        /// Tạo URL thanh toán VNPAY từ thông tin đơn hàng
        /// </summary>
        /// <param name="maLopHP">Mã lớp học phần (dùng làm mã đơn hàng)</param>
        /// <param name="amount">Số tiền thanh toán (VNĐ)</param>
        /// <param name="orderInfo">Thông tin đơn hàng (nội dung chuyển khoản)</param>
        /// <param name="ipAddress">IP của người dùng</param>
        /// <returns>URL redirect đến trang thanh toán VNPAY</returns>
        public static string CreatePaymentUrl(string maLopHP, long amount, string orderInfo, string ipAddress)
        {
            // Mã giao dịch duy nhất: MaLopHP + timestamp
            string txnRef = $"{maLopHP}_{DateTime.Now:yyyyMMddHHmmss}";

            var vnpParams = new SortedDictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {
                { "vnp_Version", Version },
                { "vnp_Command", Command },
                { "vnp_TmnCode", TmnCode },
                { "vnp_Amount", (amount * 100).ToString() }, // VNPAY yêu cầu * 100
                { "vnp_CurrCode", CurrCode },
                { "vnp_TxnRef", txnRef },
                { "vnp_OrderInfo", orderInfo },
                { "vnp_OrderType", "other" },
                { "vnp_Locale", Locale },
                { "vnp_ReturnUrl", ReturnUrl },
                { "vnp_IpAddr", ipAddress },
                { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
                { "vnp_ExpireDate", DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss") }
            };

            // Build query string và tạo chữ ký HMAC-SHA512
            string queryString = BuildQueryString(vnpParams, false);
            string signature = HmacSHA512(HashSecret, queryString);

            return $"{PaymentUrl}?{queryString}&vnp_SecureHash={signature}";
        }

        /// <summary>
        /// Xác thực chữ ký từ VNPAY callback (tránh giả mạo giao dịch)
        /// </summary>
        public static bool ValidateSignature(IEnumerable<KeyValuePair<string, string>> queryParams)
        {
            // Lấy giá trị chữ ký từ VNPAY
            var paramDict = queryParams.ToDictionary(kv => kv.Key, kv => kv.Value);
            if (!paramDict.ContainsKey("vnp_SecureHash"))
                return false;

            string secureHash = paramDict["vnp_SecureHash"];

            // Loại bỏ các param chữ ký khỏi dữ liệu cần hash
            var filteredParams = new SortedDictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var kv in paramDict)
            {
                if (!kv.Key.Equals("vnp_SecureHash", StringComparison.InvariantCultureIgnoreCase) &&
                    !kv.Key.Equals("vnp_SecureHashType", StringComparison.InvariantCultureIgnoreCase))
                {
                    filteredParams[kv.Key] = kv.Value;
                }
            }

            string queryString = BuildQueryString(filteredParams, false);
            string expectedHash = HmacSHA512(HashSecret, queryString);

            return expectedHash.Equals(secureHash, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Lấy IP của người dùng từ HttpRequest
        /// </summary>
        public static string GetClientIp(HttpRequestBase request)
        {
            string ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
                ip = request.ServerVariables["REMOTE_ADDR"];
            if (string.IsNullOrEmpty(ip))
                ip = request.UserHostAddress;
            // Nếu có nhiều IP (proxy chain), lấy IP đầu tiên
            if (!string.IsNullOrEmpty(ip) && ip.Contains(","))
                ip = ip.Split(',')[0].Trim();
            return ip ?? "127.0.0.1";
        }

        // =====================================================================
        // PRIVATE HELPERS
        // =====================================================================

        private static string BuildQueryString(SortedDictionary<string, string> parameters, bool encode)
        {
            var sb = new StringBuilder();
            foreach (var kv in parameters)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    if (sb.Length > 0) sb.Append('&');
                    sb.Append(kv.Key);
                    sb.Append('=');
                    sb.Append(encode ? HttpUtility.UrlEncode(kv.Value) : kv.Value);
                }
            }
            return sb.ToString();
        }

        private static string HmacSHA512(string key, string inputData)
        {
            var hash = new StringBuilder();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                foreach (byte theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }
            return hash.ToString();
        }
    }
}

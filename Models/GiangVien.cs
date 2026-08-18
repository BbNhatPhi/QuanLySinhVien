using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace StudentManagementSystem.Models
{
    public class GiangVien
    {
        public string MaGV { get; set; }
        public string HoTen { get; set; }
        public string Email { get; set; }
        public string SoDienThoai { get; set; }
        public int KhoaID { get; set; }
        public string TenKhoa { get; set; }
        public string MaMon { get; set; }
        public string TenMon { get; set; }
    }
}
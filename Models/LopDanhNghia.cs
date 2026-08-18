using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace StudentManagementSystem.Models
{
    public class LopDanhNghia
    {
        public string MaLop { get; set; }
        public string TenLop { get; set; }
        public int NganhID { get; set; }
        public string TenNganh { get; set; } // Lấy từ lệnh JOIN để hiển thị ra View
        public string KhoaHoc { get; set; }
    }
}
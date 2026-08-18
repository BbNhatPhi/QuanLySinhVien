using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentManagementSystem.Models
{
    public class Nganh
    {
        public int NganhID { get; set; }
        public string TenNganh { get; set; }
        public int KhoaID { get; set; }
        public string TenKhoa { get; set; } // Thuộc tính này để chứa Tên khoa khi JOIN 2 bảng
    }
}
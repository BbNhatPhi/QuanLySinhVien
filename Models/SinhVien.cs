using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace StudentManagementSystem.Models
{
    public class SinhVien
    {
        public string MaSV { get; set; }
        public string HoTen { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public string Email { get; set; }
        public string SoDienThoai { get; set; }
        public string TenNganh { get; set; } // JOIN từ bảng Nganh
        public string TrangThaiHocTap { get; set; }
        public string MaNganh { get; set; }
        public int KhoaID { get; set; }
        public string TenKhoa { get; set; }
    }
}
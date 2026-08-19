using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace StudentManagementSystem.Models
{
    public class YeuCauHanhChinh
    {
        public int MaYeuCau { get; set; }
        public string LoaiDichVu { get; set; }
        public string LyDo { get; set; }
        public DateTime NgayGui { get; set; }
        public string TrangThai { get; set; }
        public int MaYC { get; set; }
        public int SoLuong { get; set; }
        public string MaSV { get; set; }
        public string HoTen { get; set; } // Lấy từ bảng SinhVien
        public string MoTa { get; set; }
    }
}
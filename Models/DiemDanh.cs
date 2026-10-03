using System;
namespace StudentManagementSystem.Models {
    public class DiemDanh {
        public int DiemDanhID { get; set; }
        public string MaLopHP { get; set; }
        public string MaSV { get; set; }
        public string HoTenSV { get; set; }
        public DateTime NgayHoc { get; set; }
        public string TrangThai { get; set; }
        public string GhiChu { get; set; }
    }
}
using System;

namespace StudentManagementSystem.Models
{
    public class ChuongTrinhKhungItem
    {
        public string MaMon { get; set; }
        public string TenMon { get; set; }
        public int SoTinChi { get; set; }
        public int HocKyTieuChuan { get; set; }
        public string TrangThai { get; set; } // Đã học (Qua môn / Rớt), Đang học, Chưa học
    }
}

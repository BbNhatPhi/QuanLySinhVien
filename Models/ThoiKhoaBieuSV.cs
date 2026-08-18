using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentManagementSystem.Models
{
    public class ThoiKhoaBieuSV
    {
        public string TenMon { get; set; }
        public string MaLopHP { get; set; }
        public int NgayHoc { get; set; } // Thứ mấy
        public int CaHoc { get; set; } // Sáng, Chiều, Tối
        public string PhongHoc { get; set; }
        public string TenGiangVien { get; set; }
    }
}
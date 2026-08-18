using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace StudentManagementSystem.Models
{
    public class LichDay
    {
        public string MaLopHP { get; set; } // Đổi từ MaLopTC sang MaLopHP
        public string TenMon { get; set; }
        public string PhongHoc { get; set; }

        // Bổ sung thêm Ngày và Ca học để hiển thị cho GV
        public int NgayHoc { get; set; }
        public int CaHoc { get; set; }

        public string TenNgayHoc => NgayHoc == 8 ? "Chủ nhật" : "Thứ " + NgayHoc;
        public string TenCaHoc => CaHoc == 1 ? "Sáng" : (CaHoc == 2 ? "Chiều" : "Tối");
    }
}
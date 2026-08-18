using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace StudentManagementSystem.Models
{
    public class ThoiKhoaBieu
    {
        public int MaTKB { get; set; }
        public string MaLopHP { get; set; }
        public string TenMon { get; set; }
        public string TenGV { get; set; }
        public int NgayHoc { get; set; }
        public int CaHoc { get; set; }
        public string PhongHoc { get; set; }

        // Code xử lý thêm để hiển thị chữ cho đẹp (VD: 2 -> "Thứ 2", 1 -> "Ca Sáng")
        public string TenNgayHoc => NgayHoc == 8 ? "Chủ nhật" : "Thứ " + NgayHoc;
        public string TenCaHoc => CaHoc == 1 ? "Sáng" : (CaHoc == 2 ? "Chiều" : "Tối");
    }
}
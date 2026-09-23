using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace StudentManagementSystem.Models
{
    public class LopHocPhan
    {
        public string MaLopHP { get; set; }
        public string MaMon { get; set; }
        public string TenMon { get; set; }
        public string MaGV { get; set; }
        public string TenGV { get; set; }
        public int HocKy { get; set; }
        public string NamHoc { get; set; }
        public int SoLuongMax { get; set; }
        public string TrangThai { get; set; }
        public int SoTinChi { get; set; }
        public bool DaThanhToan { get; set; }
    }
}
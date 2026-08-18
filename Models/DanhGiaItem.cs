using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace StudentManagementSystem.Models
{
    public class DanhGiaItem
    {
        public string MaLopHP { get; set; }
        public string TenMon { get; set; }
        public string TenGV { get; set; }
        public bool DaDanhGia { get; set; }
        public int DiemDanhGia { get; set; }
        public string NhanXet { get; set; }
    }
}
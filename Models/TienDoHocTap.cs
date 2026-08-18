using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace StudentManagementSystem.Models
{
    public class TienDoHocTap
    {
        public int TinChiTichLuy { get; set; }
        public int TongTinChiDaHoc { get; set; }
        public double GPA_He10 { get; set; }
        public double GPA_He4 { get; set; }
        public int SoMonNo { get; set; }
        public int TongTinChiYeuCau { get; set; } = 150;
    }
}
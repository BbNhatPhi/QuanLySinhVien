using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentManagementSystem.Models
{
    public class KetQuaHocTap
    {
        public string TenMon { get; set; }
        public int SoTinChi { get; set; }
        public int HocKy { get; set; }
        public string NamHoc { get; set; }
        public double DiemCC { get; set; }
        public double DiemGK { get; set; }
        public double DiemCK { get; set; }
        public double DiemTong { get; set; }
    }
}
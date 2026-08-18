using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentManagementSystem.Models
{
    public class SinhVienDiem
    {
        public int MaDiem { get; set; }
        public string MaSV { get; set; }
        public string HoTen { get; set; }
        public double DiemCC { get; set; }
        public double DiemGK { get; set; }
        public double DiemCK { get; set; }
        public double DiemTong { get; set; }
    }
}
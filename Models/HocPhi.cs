using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace StudentManagementSystem.Models
{
    public class HocPhi
    {
        public string MaLopHP { get; set; }
        public string TenMon { get; set; }
        public int SoTinChi { get; set; }
        public double DonGia { get; set; }

        // Tự động tính thành tiền cho từng môn
        public double ThanhTien => SoTinChi * DonGia;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace StudentManagementSystem.Models
{
    public class LichThi
    {
        public int MaLichThi { get; set; }
        public string MaLopHP { get; set; }
        public string TenMon { get; set; }
        public DateTime NgayThi { get; set; }
        public int CaThi { get; set; }
        public string PhongThi { get; set; }
    }
}
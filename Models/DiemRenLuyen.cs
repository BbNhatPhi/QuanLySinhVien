using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace StudentManagementSystem.Models
{
    public class DiemRenLuyen
    {
        public int ID { get; set; }
        public string MaSV { get; set; }
        public string HocKy { get; set; }
        public int Diem { get; set; }
        public string XepLoai { get; set; }
    }
}
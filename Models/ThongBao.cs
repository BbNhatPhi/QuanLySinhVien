using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace StudentManagementSystem.Models
{
    public class ThongBao
    {
        public int MaTB { get; set; }
        public string TieuDe { get; set; }
        public string NoiDung { get; set; }
        public DateTime NgayDang { get; set; }
        public string NguoiDang { get; set; }
        public string DoiTuong { get; set; }
    }
}
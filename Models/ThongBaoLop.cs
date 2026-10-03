using System;
namespace StudentManagementSystem.Models {
    public class ThongBaoLop {
        public int ThongBaoID { get; set; }
        public string MaLopHP { get; set; }
        public string TieuDe { get; set; }
        public string NoiDung { get; set; }
        public DateTime NgayDang { get; set; }
    }
}
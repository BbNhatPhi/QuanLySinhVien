using System;

namespace StudentManagementSystem.Models
{
    public class SystemLog
    {
        public int LogID { get; set; }
        public string Username { get; set; }
        public string Action { get; set; }
        public DateTime CreatedAt { get; set; }
        public string IPAddress { get; set; }
    }
}
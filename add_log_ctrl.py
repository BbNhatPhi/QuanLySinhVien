import codecs
import re

with codecs.open('Controllers/AdminController.cs', 'r', 'utf-8-sig') as f:
    text = f.read()

# Add SystemLogs action
new_action = '''
        public ActionResult SystemLogs()
        {
            List<StudentManagementSystem.Models.SystemLog> logs = new List<StudentManagementSystem.Models.SystemLog>();
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string sql = "SELECT TOP 100 * FROM SystemLogs ORDER BY CreatedAt DESC";
                System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand(sql, conn);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        logs.Add(new StudentManagementSystem.Models.SystemLog
                        {
                            LogID = Convert.ToInt32(reader["LogID"]),
                            Username = reader["Username"].ToString(),
                            Action = reader["Action"].ToString(),
                            CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                            IPAddress = reader["IPAddress"] != DBNull.Value ? reader["IPAddress"].ToString() : ""
                        });
                    }
                }
            }
            return View(logs);
        }
'''

if 'SystemLogs()' not in text:
    # Insert before last closing brace of class
    idx = text.rfind('}')
    idx = text.rfind('}', 0, idx)
    text = text[:idx] + new_action + text[idx:]
    with codecs.open('Controllers/AdminController.cs', 'w', 'utf-8-sig') as f:
        f.write(text)
    print("Added SystemLogs to AdminController")
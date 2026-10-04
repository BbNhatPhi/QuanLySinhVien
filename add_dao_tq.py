import codecs
import re

with codecs.open('DAO/StaffDAO.cs', 'r', 'utf-8-sig') as f:
    text = f.read()

new_methods = '''
        public List<MonHoc> GetMonTienQuyet(string maMon)
        {
            List<MonHoc> list = new List<MonHoc>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string sql = "SELECT m.* FROM MonTienQuyet mtq INNER JOIN MonHoc m ON mtq.MaMonTQ = m.MaMon WHERE mtq.MaMon = @MaMon";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaMon", maMon);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new MonHoc
                        {
                            MaMon = reader["MaMon"].ToString(),
                            TenMon = reader["TenMon"].ToString(),
                            SoTinChi = Convert.ToInt32(reader["SoTinChi"])
                        });
                    }
                }
            }
            return list;
        }

        public string AddMonTienQuyet(string maMon, string maMonTQ)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    string sql = "INSERT INTO MonTienQuyet (MaMon, MaMonTQ) VALUES (@MaMon, @MaMonTQ)";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@MaMon", maMon);
                    cmd.Parameters.AddWithValue("@MaMonTQ", maMonTQ);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    return "Success";
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Violation of PRIMARY KEY")) return "Môn này đã là tiên quyết rồi!";
                    return ex.Message;
                }
            }
        }

        public string RemoveMonTienQuyet(string maMon, string maMonTQ)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    string sql = "DELETE FROM MonTienQuyet WHERE MaMon = @MaMon AND MaMonTQ = @MaMonTQ";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@MaMon", maMon);
                    cmd.Parameters.AddWithValue("@MaMonTQ", maMonTQ);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    return "Success";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }
'''

if 'GetMonTienQuyet' not in text:
    # Insert before last two closing braces
    idx = text.rfind('}')
    idx = text.rfind('}', 0, idx)
    text = text[:idx] + new_methods + text[idx:]
    with codecs.open('DAO/StaffDAO.cs', 'w', 'utf-8-sig') as f:
        f.write(text)
    print("Added Prerequisite methods to StaffDAO")
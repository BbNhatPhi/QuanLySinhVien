import codecs

with codecs.open('DAO/StudentDAO.cs', 'r', 'utf-8-sig') as f:
    text = f.read()

import re

# Update GetThongTinCaNhan mapping
old_mapping = r'''                            sv = new SinhVien
                            \{
                                MaSV = reader\["MaSV"\].ToString\(\),
                                HoTen = reader\["HoTen"\].ToString\(\),
                                GioiTinh = reader\["GioiTinh"\].ToString\(\),
                                NgaySinh = reader\["NgaySinh"\] != DBNull\.Value \? \(DateTime\?\)Convert\.ToDateTime\(reader\["NgaySinh"\]\) : null,
                                Email = reader\["Email"\].ToString\(\),
                                SoDienThoai = reader\["SoDienThoai"\] != DBNull\.Value \? reader\["SoDienThoai"\].ToString\(\) : "",
                                DiaChi = reader\["DiaChi"\] != DBNull\.Value \? reader\["DiaChi"\].ToString\(\) : ""
                            \};'''

new_mapping = r'''                            sv = new SinhVien
                            {
                                MaSV = reader["MaSV"].ToString(),
                                HoTen = reader["HoTen"].ToString(),
                                GioiTinh = reader["GioiTinh"].ToString(),
                                NgaySinh = reader["NgaySinh"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["NgaySinh"]) : null,
                                Email = reader["Email"].ToString(),
                                SoDienThoai = reader["SoDienThoai"] != DBNull.Value ? reader["SoDienThoai"].ToString() : "",
                                DiaChi = reader["DiaChi"] != DBNull.Value ? reader["DiaChi"].ToString() : "",
                                TenNganh = reader["TenNganh"] != DBNull.Value ? reader["TenNganh"].ToString() : "",
                                MaNganh = reader["NganhID"] != DBNull.Value ? reader["NganhID"].ToString() : ""
                            };'''

text = re.sub(old_mapping, new_mapping, text)

with codecs.open('DAO/StudentDAO.cs', 'w', 'utf-8-sig') as f:
    f.write(text)

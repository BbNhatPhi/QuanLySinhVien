import codecs

with codecs.open('Views/Student/TienDoHocTap.cshtml', 'r', 'utf-8-sig') as f:
    text = f.read()

# Replace the OrderBy and the loop block
import re

# 1. Replace the OrderBy
text = re.sub(r'bangDiem\.OrderBy\(x => x\.TrangThai\)\.ThenBy\(x => x\.TenMon\)', 'bangDiem.OrderByDescending(x => x.NamHoc).ThenByDescending(x => x.HocKy).ThenBy(x => x.TenMon)', text)

# 2. Replace the table rows inside the loop
old_tr = r'''                        <tr>
                            <td class="text-muted fw-bold">@stt</td>
                            <td class="fw-bold text-dark">@bd\.TenMon</td>
                            <td><span class="badge bg-light text-dark border">@bd\.MaLopHP</span></td>
                            <td class="text-center fw-bold">3</td> <!-- Hardcode TC for now -->
                            <td class="text-center fw-bold @\(bd\.Diem10 >= 5 \? "text-primary" : \(bd\.Diem10 > 0 \? "text-danger" : ""\)\)">
                                @\(bd\.Diem10 > 0 \? bd\.Diem10\.ToString\("0\.0"\) : "-"\)
                            </td>
                            <td class="text-center fw-bold">@\(bd\.Diem4 > 0 \? bd\.Diem4\.ToString\("0\.0"\) : "-"\)</td>
                            <td class="text-center fw-bold">
                                @if\(bd\.DiemChu == "A" \|\| bd\.DiemChu == "B" \|\| bd\.DiemChu == "C"\) \{
                                    <span class="text-success">@bd\.DiemChu</span>
                                \} else if \(bd\.DiemChu == "D"\) \{
                                    <span class="text-warning">@bd\.DiemChu</span>
                                \} else if \(bd\.DiemChu == "F"\) \{
                                    <span class="text-danger">@bd\.DiemChu</span>
                                \} else \{
                                    <span>-</span>
                                \}
                            </td>
                            <td class="text-center">
                                @if \(bd\.TrangThai == "Đạt"\) \{
                                    <span class="badge bg-success-subtle text-success border border-success"><i class="fas fa-check me-1"></i>Qua môn</span>
                                \} else if \(bd\.TrangThai == "Học Lại" \|\| bd\.TrangThai == "Rớt"\) \{
                                    <span class="badge bg-danger-subtle text-danger border border-danger"><i class="fas fa-times me-1"></i>Học lại</span>
                                \} else \{
                                    <span class="badge bg-info-subtle text-info border border-info"><i class="fas fa-sync fa-spin me-1"></i>Đang học</span>
                                \}
                            </td>
                        </tr>'''

new_tr = '''                        <tr>
                            @{
                                double d10 = bd.DiemTong;
                                double d4 = 0;
                                string dChu = "F";
                                string tThai = "Rớt";
                                
                                if (d10 >= 8.5) { d4 = 4.0; dChu = "A"; tThai = "Đạt"; }
                                else if (d10 >= 7.0) { d4 = 3.0; dChu = "B"; tThai = "Đạt"; }
                                else if (d10 >= 5.5) { d4 = 2.0; dChu = "C"; tThai = "Đạt"; }
                                else if (d10 >= 4.0) { d4 = 1.0; dChu = "D"; tThai = "Đạt"; }
                                else if (d10 > 0 || bd.DiemCC > 0 || bd.DiemGK > 0) { d4 = 0; dChu = "F"; tThai = "Rớt"; }
                                else { d4 = 0; dChu = "-"; tThai = "Đang học"; }
                            }
                            <td class="text-muted fw-bold">@stt</td>
                            <td class="fw-bold text-dark">@bd.TenMon</td>
                            <td><span class="badge bg-light text-dark border">HK @bd.HocKy - @bd.NamHoc</span></td>
                            <td class="text-center fw-bold">@bd.SoTinChi</td>
                            <td class="text-center fw-bold @(d10 >= 5 ? "text-primary" : (d10 > 0 ? "text-danger" : ""))">
                                @(d10 > 0 ? d10.ToString("0.0") : "-")
                            </td>
                            <td class="text-center fw-bold">@(d4 > 0 ? d4.ToString("0.0") : "-")</td>
                            <td class="text-center fw-bold">
                                @if(dChu == "A" || dChu == "B" || dChu == "C") {
                                    <span class="text-success">@dChu</span>
                                } else if (dChu == "D") {
                                    <span class="text-warning">@dChu</span>
                                } else if (dChu == "F") {
                                    <span class="text-danger">@dChu</span>
                                } else {
                                    <span>-</span>
                                }
                            </td>
                            <td class="text-center">
                                @if (tThai == "Đạt") {
                                    <span class="badge bg-success-subtle text-success border border-success"><i class="fas fa-check me-1"></i>Qua môn</span>
                                } else if (tThai == "Rớt") {
                                    <span class="badge bg-danger-subtle text-danger border border-danger"><i class="fas fa-times me-1"></i>Học lại</span>
                                } else {
                                    <span class="badge bg-info-subtle text-info border border-info"><i class="fas fa-sync fa-spin me-1"></i>Đang học</span>
                                }
                            </td>
                        </tr>'''

text = re.sub(old_tr, new_tr, text)

# Also fix the header "Lớp HP" to "Học kỳ"
text = text.replace('<th>Lớp HP</th>', '<th>Học Kỳ</th>')

with codecs.open('Views/Student/TienDoHocTap.cshtml', 'w', 'utf-8-sig') as f:
    f.write(text)

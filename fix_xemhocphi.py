import codecs

with codecs.open('Views/Student/XemHocPhi.cshtml', 'r', 'utf-8-sig') as f:
    text = f.read()

# Fix type and property name
text = text.replace('decimal tongNo = Model != null ? Model.Sum(m => m.SoTien) : 0;', 'double tongNo = Model != null ? Model.Sum(m => m.ThanhTien) : 0;')
text = text.replace('decimal daNop = lichSuGiaoDich != null ? lichSuGiaoDich.Sum(m => m.SoTien) : 0;', 'double daNop = lichSuGiaoDich != null ? lichSuGiaoDich.Sum(m => m.ThanhTien) : 0;')

text = text.replace('@item.SoTien.ToString("N0")', '@item.ThanhTien.ToString("N0")')

old_btn = 'new { maLopHP = item.MaLopHP }'
new_btn = 'new { maLopHP = item.MaLopHP, tenMon = item.TenMon, soTien = item.ThanhTien }'
text = text.replace(old_btn, new_btn)

with codecs.open('Views/Student/XemHocPhi.cshtml', 'w', 'utf-8-sig') as f:
    f.write(text)
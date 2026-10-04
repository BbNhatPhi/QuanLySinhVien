import codecs

with codecs.open('Views/Student/ThanhToan.cshtml', 'r', 'utf-8-sig') as f:
    text = f.read()

old_input = '<input type="hidden" name="maLopHP" value="@ViewBag.MaLopHP" />'
new_input = '<input type="hidden" name="maLopHP" value="@ViewBag.MaLopHP" />\n                <input type="hidden" name="soTien" value="@ViewBag.SoTien" />'

text = text.replace(old_input, new_input)

with codecs.open('Views/Student/ThanhToan.cshtml', 'w', 'utf-8-sig') as f:
    f.write(text)
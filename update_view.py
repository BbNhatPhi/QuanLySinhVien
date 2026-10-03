import codecs

with codecs.open('Views/Teacher/DiemDanh.cshtml', 'r', 'utf-8-sig') as f:
    text = f.read()

text = text.replace('Url.Action("LuuDiemDanh", "Teacher")', 'Url.Action("LuuDiemDanhNgay", "Teacher")')

with codecs.open('Views/Teacher/DiemDanh.cshtml', 'w', 'utf-8-sig') as f:
    f.write(text)

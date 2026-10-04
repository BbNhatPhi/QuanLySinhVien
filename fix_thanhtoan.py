import codecs

with codecs.open('Views/Student/ThanhToan.cshtml', 'r', 'utf-8-sig') as f:
    text = f.read()

text = text.replace('XuLyThanhToan', 'XacNhanThanhToan')

with codecs.open('Views/Student/ThanhToan.cshtml', 'w', 'utf-8-sig') as f:
    f.write(text)
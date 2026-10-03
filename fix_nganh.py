import codecs

with codecs.open('Views/Student/Index.cshtml', 'r', 'utf-8-sig') as f:
    text = f.read()

text = text.replace('Công nghệ thông tin</div>', '@(thongTin?.TenNganh ?? "Công nghệ thông tin")</div>')

with codecs.open('Views/Student/Index.cshtml', 'w', 'utf-8-sig') as f:
    f.write(text)

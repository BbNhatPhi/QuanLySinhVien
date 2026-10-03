import codecs

with codecs.open('Views/Student/Index.cshtml', 'r', 'utf-8-sig') as f:
    text = f.read()

text = text.replace('Url.Action("ThoiKhoaBieu", "Student")', 'Url.Action("TKB", "Student")')

with codecs.open('Views/Student/Index.cshtml', 'w', 'utf-8-sig') as f:
    f.write(text)

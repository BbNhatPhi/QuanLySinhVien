import codecs

with codecs.open('Views/Student/TienDoHocTap.cshtml', 'r', 'utf-8-sig') as f:
    text = f.read()

text = text.replace('@keyframes', '@@keyframes')

with codecs.open('Views/Student/TienDoHocTap.cshtml', 'w', 'utf-8-sig') as f:
    f.write(text)

import codecs

with codecs.open('Views/Admin/SystemLogs.cshtml', 'r', 'utf-8-sig') as f:
    text = f.read()

text = text.replace('Layout = "~/Views/Shared/_AdminLayout.cshtml";', 'Layout = "~/Views/Shared/_Layout.cshtml";')

with codecs.open('Views/Admin/SystemLogs.cshtml', 'w', 'utf-8-sig') as f:
    f.write(text)
import codecs
import re

with codecs.open('Views/Student/Index.cshtml', 'r', 'utf-8-sig') as f:
    text = f.read()

# Remove the Action Box for DanhGiaGiangVien
text = re.sub(r'<a href="@Url\.Action\("DanhGiaGiangVien".*?</a>', '', text, flags=re.DOTALL)

# Also replace the empty string that might have been left if there was surrounding whitespace
with codecs.open('Views/Student/Index.cshtml', 'w', 'utf-8-sig') as f:
    f.write(text)

with codecs.open('Views/Shared/_Layout.cshtml', 'r', 'utf-8-sig') as f:
    layout = f.read()

layout = re.sub(r'<a href="@Url\.Action\("DanhGiaGiangVien".*?</a>', '', layout, flags=re.DOTALL)

with codecs.open('Views/Shared/_Layout.cshtml', 'w', 'utf-8-sig') as f:
    f.write(layout)

import codecs
import re

with codecs.open('Controllers/StudentController.cs', 'r', 'utf-8-sig') as f:
    text = f.read()

# Just use simpler regexes
text = re.sub(r'int soMonChuaDanhGia[\s\S]*?RedirectToAction\("DanhGiaGiangVien"\);\s*\}', '', text)

with codecs.open('Controllers/StudentController.cs', 'w', 'utf-8-sig') as f:
    f.write(text)

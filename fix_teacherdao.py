import codecs
with codecs.open('DAO/TeacherDAO.cs', 'r', 'utf-8-sig') as f:
    text = f.read()

import re
text = re.sub(r"sv\.TrangThaiHocTap = N'.*?'", "sv.TrangThaiHocTap = N'Đang học'", text)

with codecs.open('DAO/TeacherDAO.cs', 'w', 'utf-8-sig') as f:
    f.write(text)

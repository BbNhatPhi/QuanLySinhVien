import codecs

with codecs.open('Controllers/StudentController.cs', 'r', 'utf-8-sig') as f:
    text = f.read()

text = text.replace('_studentDAO.GetLopHocPhanMoDangKy()', '_studentDAO.GetLopHocPhanMoDangKy(User.Identity.Name)')

with codecs.open('Controllers/StudentController.cs', 'w', 'utf-8-sig') as f:
    f.write(text)

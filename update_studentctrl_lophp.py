import codecs

with codecs.open('Controllers/StudentController.cs', 'r', 'utf-8-sig') as f:
    text = f.read()

text = text.replace('var dsLop = _studentDAO.GetLopHocPhanMoDangKy();', 'string username = User.Identity.Name;\n            var dsLop = _studentDAO.GetLopHocPhanMoDangKy(username);')

with codecs.open('Controllers/StudentController.cs', 'w', 'utf-8-sig') as f:
    f.write(text)

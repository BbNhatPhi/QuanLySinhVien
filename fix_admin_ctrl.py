import codecs

with codecs.open('Controllers/AdminController.cs', 'r', 'utf-8-sig') as f:
    text = f.read()

if 'using System.Configuration;' not in text:
    text = text.replace('using System.Web.Mvc;', 'using System.Web.Mvc;\nusing System.Configuration;')
    with codecs.open('Controllers/AdminController.cs', 'w', 'utf-8-sig') as f:
        f.write(text)
    print("Added using System.Configuration;")
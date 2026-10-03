import codecs

with codecs.open('Controllers/StudentController.cs', 'r', 'utf-8-sig') as f:
    text = f.read()

text = text.replace('public ActionResult ThoiKhoaBieu()', 'public ActionResult TKB()')
text = text.replace('return View("ThoiKhoaBieu", tkb);', 'return View("TKB", tkb);')

with codecs.open('Controllers/StudentController.cs', 'w', 'utf-8-sig') as f:
    f.write(text)

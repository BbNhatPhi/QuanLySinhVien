import codecs

with codecs.open('Controllers/AccountController.cs', 'r', 'utf-8-sig') as f:
    text = f.read()

text = text.replace('string emailMock = $"[M', 'string emailMock = $@"[M')

with codecs.open('Controllers/AccountController.cs', 'w', 'utf-8-sig') as f:
    f.write(text)
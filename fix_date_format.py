import codecs
import re

with codecs.open('Views/Student/Index.cshtml', 'r', 'utf-8-sig') as f:
    text = f.read()

# Replace the problematic line
text = text.replace(
    'thongTin?.NgaySinh.ToString("dd/MM/yyyy")', 
    'thongTin?.NgaySinh?.ToString("dd/MM/yyyy")'
)

with codecs.open('Views/Student/Index.cshtml', 'w', 'utf-8-sig') as f:
    f.write(text)

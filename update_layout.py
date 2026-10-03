import codecs

with codecs.open('Views/Shared/_Layout.cshtml', 'r', 'utf-8-sig') as f:
    text = f.read()

old_link = '<a href="@Url.Action("Index","Student")" class="menu-link @Nav("Student","Index")"><i class="fas fa-calendar-days"></i> Thời khóa biểu</a>'
new_links = '''<a href="@Url.Action("Index","Student")" class="menu-link @Nav("Student","Index")"><i class="fas fa-home"></i> Trang chủ</a>
                    <a href="@Url.Action("TKB","Student")" class="menu-link @Nav("Student","TKB")"><i class="fas fa-calendar-days"></i> Thời khóa biểu</a>'''

text = text.replace(old_link, new_links)

# Also handle cases where there might be encoding differences in the original file
import re
text = re.sub(
    r'<a href="@Url\.Action\("Index","Student"\)".*?>\s*<i.*?></i>.*?khóa biểu\s*</a>', 
    new_links, 
    text
)

with codecs.open('Views/Shared/_Layout.cshtml', 'w', 'utf-8-sig') as f:
    f.write(text)

import codecs
import re

with codecs.open('Views/Shared/_Layout.cshtml', 'r', 'utf-8-sig') as f:
    text = f.read()

old_nav = '<a href="@Url.Action("ManagePhongHoc","Admin")" class="menu-link @Nav("Admin","ManagePhongHoc")"><i class="fas fa-chalkboard-user"></i> Phòng học</a>'
new_nav = old_nav + '\n                    <a href="@Url.Action("SystemLogs","Admin")" class="menu-link @Nav("Admin","SystemLogs")"><i class="fas fa-history"></i> Lịch sử hệ thống</a>'

if 'SystemLogs' not in text:
    text = text.replace(old_nav, new_nav)
    with codecs.open('Views/Shared/_Layout.cshtml', 'w', 'utf-8-sig') as f:
        f.write(text)
    print("Added log link to Layout")
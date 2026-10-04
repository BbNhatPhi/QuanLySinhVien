import codecs

with codecs.open('Views/Shared/_AdminLayout.cshtml', 'r', 'utf-8-sig') as f:
    text = f.read()

nav_item = '''                        <li class="nav-item">
                            <a class="nav-link text-white @(ViewContext.RouteData.Values["action"].ToString() == "BackupData" ? "active" : "")" href="@Url.Action("BackupData", "Admin")">
                                <i class="fas fa-database me-2"></i>Sao lưu dữ liệu
                            </a>
                        </li>'''

new_nav = nav_item + '''
                        <li class="nav-item">
                            <a class="nav-link text-white @(ViewContext.RouteData.Values["action"].ToString() == "SystemLogs" ? "active" : "")" href="@Url.Action("SystemLogs", "Admin")">
                                <i class="fas fa-history me-2"></i>Lịch sử hệ thống
                            </a>
                        </li>'''

if 'SystemLogs' not in text:
    text = text.replace(nav_item, new_nav)
    with codecs.open('Views/Shared/_AdminLayout.cshtml', 'w', 'utf-8-sig') as f:
        f.write(text)
    print("Added link to _AdminLayout")
import codecs

with codecs.open('Views/Student/Index.cshtml', 'r', 'utf-8-sig') as f:
    text = f.read()

# Update CSS for stat-box to support being a link
css_old = '''    .stat-box {
        border-radius: 8px;
        padding: 20px;
        color: #333;
        display: flex;
        flex-direction: column;
        justify-content: space-between;
        min-height: 120px;
    }'''

css_new = '''    .stat-box {
        border-radius: 8px;
        padding: 20px;
        color: #333;
        display: flex;
        flex-direction: column;
        justify-content: space-between;
        min-height: 120px;
        text-decoration: none;
        transition: transform 0.2s, box-shadow 0.2s;
    }
    .stat-box:hover {
        transform: translateY(-3px);
        box-shadow: 0 4px 12px rgba(0,0,0,0.08);
        color: #333;
    }'''
text = text.replace(css_old, css_new)

# Update HTML blocks
notify_old = '''<div class="stat-box notify">
                    <div class="title">Nhắc nhở mới, chưa xem</div>
                    <div class="number text-dark">@soThongBao</div>
                    <a href="@Url.Action("XemThongBao", "Student")" class="link">Xem chi tiết <i class="far fa-bell"></i></a>
                </div>'''
notify_new = '''<a href="@Url.Action("XemThongBao", "Student")" class="stat-box notify text-decoration-none">
                    <div class="title">Nhắc nhở mới, chưa xem</div>
                    <div class="number text-dark">@soThongBao</div>
                    <div class="link">Xem chi tiết <i class="far fa-bell"></i></div>
                </a>'''
text = text.replace(notify_old, notify_new)

schedule_old = '''<div class="stat-box schedule">
                    <div class="title">Lịch học trong tuần</div>
                    <div class="number text-primary">@soLichHoc</div>
                    <a href="@Url.Action("TKB", "Student")" class="link">Xem chi tiết <i class="far fa-calendar-alt"></i></a>
                </div>'''
schedule_new = '''<a href="@Url.Action("TKB", "Student")" class="stat-box schedule text-decoration-none">
                    <div class="title">Lịch học trong tuần</div>
                    <div class="number text-primary">@soLichHoc</div>
                    <div class="link">Xem chi tiết <i class="far fa-calendar-alt"></i></div>
                </a>'''
text = text.replace(schedule_old, schedule_new)

exam_old = '''<div class="stat-box exam">
                    <div class="title">Lịch thi trong tuần</div>
                    <div class="number text-warning">@soLichThi</div>
                    <a href="@Url.Action("XemLichThi", "Student")" class="link">Xem chi tiết <i class="far fa-calendar-check"></i></a>
                </div>'''
exam_new = '''<a href="@Url.Action("XemLichThi", "Student")" class="stat-box exam text-decoration-none">
                    <div class="title">Lịch thi trong tuần</div>
                    <div class="number text-warning">@soLichThi</div>
                    <div class="link">Xem chi tiết <i class="far fa-calendar-check"></i></div>
                </a>'''
text = text.replace(exam_old, exam_new)

with codecs.open('Views/Student/Index.cshtml', 'w', 'utf-8-sig') as f:
    f.write(text)

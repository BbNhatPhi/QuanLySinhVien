import codecs

with codecs.open('Views/Student/Index.cshtml', 'r', 'utf-8-sig') as f:
    text = f.read()

# Make it completely bulletproof
safe_code = '(thongTin != null && thongTin.NgaySinh.HasValue ? thongTin.NgaySinh.Value.ToString("dd/MM/yyyy") : "N/A")'

text = text.replace(
    'thongTin?.NgaySinh?.ToString("dd/MM/yyyy") ?? "N/A"', 
    safe_code
)
text = text.replace(
    'thongTin?.NgaySinh.ToString("dd/MM/yyyy") ?? "N/A"', 
    safe_code
)

with codecs.open('Views/Student/Index.cshtml', 'w', 'utf-8-sig') as f:
    f.write(text)

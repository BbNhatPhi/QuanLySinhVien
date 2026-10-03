import codecs

with codecs.open('Views/Student/Index.cshtml', 'r', 'utf-8-sig') as f:
    text = f.read()

# Fix 1: DiaChi instead of QueQuan
text = text.replace('thongTin?.QueQuan', 'thongTin?.DiaChi')
text = text.replace('Nơi sinh:', 'Địa chỉ:')

# Fix 2: TienDoHocTap properties
text = text.replace('tienDo?.SoTinChiTichLuy', 'tienDo?.TinChiTichLuy')
text = text.replace('tienDo?.TongSoTinChiYeuCau', 'tienDo?.TongTinChiYeuCau')

with codecs.open('Views/Student/Index.cshtml', 'w', 'utf-8-sig') as f:
    f.write(text)

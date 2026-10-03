import codecs
import re

with codecs.open('Views/Student/TienDoHocTap.cshtml', 'r', 'utf-8-sig') as f:
    text = f.read()

# Replace ViewBag casting
text = text.replace('var bangDiem = ViewBag.BangDiem as List<StudentManagementSystem.Models.KetQuaHocTap>;', 'var chuongTrinhKhung = ViewBag.ChuongTrinhKhung as List<StudentManagementSystem.Models.ChuongTrinhKhungItem>;')

# Replace the table header and body
old_table = r'''    @if \(bangDiem != null && bangDiem.Count > 0\)
    \{
        <div class="table-responsive">
            <table class="table table-hover align-middle">
                <thead class="table-light text-muted small text-uppercase">
                    <tr>
                        <th style="width: 50px;">STT</th>
                        <th>Môn Học</th>
                        <th>Học Kỳ</th>
                        <th class="text-center">Tín chỉ</th>
                        <th class="text-center">Điểm Hệ 10</th>
                        <th class="text-center">Điểm Hệ 4</th>
                        <th class="text-center">Điểm Chữ</th>
                        <th class="text-center">Trạng Thái</th>
                    </tr>
                </thead>
                <tbody>[\s\S]*?</tbody>
            </table>
        </div>
    \}'''

new_table = '''    @if (chuongTrinhKhung != null && chuongTrinhKhung.Count > 0)
    {
        <div class="table-responsive">
            <table class="table table-hover align-middle">
                <thead class="table-light text-muted small text-uppercase">
                    <tr>
                        <th style="width: 50px;">STT</th>
                        <th>Mã Môn</th>
                        <th>Tên Môn Học</th>
                        <th class="text-center">Tín chỉ</th>
                        <th class="text-center">Học Kỳ Yêu Cầu</th>
                        <th class="text-center">Trạng Thái</th>
                    </tr>
                </thead>
                <tbody>
                    @{ int stt = 1; }
                    @foreach (var ctk in chuongTrinhKhung)
                    {
                        <tr>
                            <td class="text-muted fw-bold">@stt</td>
                            <td><span class="badge bg-light text-dark border">@ctk.MaMon</span></td>
                            <td class="fw-bold text-dark">@ctk.TenMon</td>
                            <td class="text-center fw-bold">@ctk.SoTinChi</td>
                            <td class="text-center fw-bold text-primary">Học kỳ @ctk.HocKyTieuChuan</td>
                            <td class="text-center">
                                @if (ctk.TrangThai == "Qua môn") {
                                    <span class="badge bg-success-subtle text-success border border-success"><i class="fas fa-check me-1"></i>Đã tích lũy</span>
                                } else if (ctk.TrangThai == "Học lại") {
                                    <span class="badge bg-danger-subtle text-danger border border-danger"><i class="fas fa-times me-1"></i>Học lại</span>
                                } else if (ctk.TrangThai == "Đang học") {
                                    <span class="badge bg-info-subtle text-info border border-info"><i class="fas fa-sync fa-spin me-1"></i>Đang học</span>
                                } else {
                                    <span class="badge bg-secondary-subtle text-secondary border border-secondary"><i class="fas fa-lock me-1"></i>Chưa học</span>
                                }
                            </td>
                        </tr>
                        stt++;
                    }
                </tbody>
            </table>
        </div>
    }'''

text = re.sub(old_table, new_table, text)

# Also fix the title of the table
text = text.replace('Chi tiết các học phần đã đăng ký', 'Chương trình khung & Kế hoạch học tập chuẩn')

with codecs.open('Views/Student/TienDoHocTap.cshtml', 'w', 'utf-8-sig') as f:
    f.write(text)

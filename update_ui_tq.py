import codecs
import re

with codecs.open('Views/Staff/ManageMonHoc.cshtml', 'r', 'utf-8-sig') as f:
    text = f.read()

# Add button
old_btn = '''                                    <!-- NÚT SỬA -->
                                    <button type="button" class="btn btn-sm btn-outline-warning fw-bold edit-monhoc-btn shadow-sm"'''

new_btn = '''                                    <!-- NÚT TIÊN QUYẾT -->
                                    <button type="button" class="btn btn-sm btn-outline-info fw-bold prereq-btn shadow-sm me-1"
                                            onclick="openPrereqModal('@item.MaMon', '@item.TenMon')">
                                        <i class="fas fa-sitemap"></i> Tiên Quyết
                                    </button>
                                    <!-- NÚT SỬA -->
                                    <button type="button" class="btn btn-sm btn-outline-warning fw-bold edit-monhoc-btn shadow-sm"'''

if 'openPrereqModal' not in text:
    text = text.replace(old_btn, new_btn)

# Add Modal & Script
prereq_modal = '''
<!-- ========================================== -->
<!-- MODAL QUẢN LÝ MÔN TIÊN QUYẾT -->
<!-- ========================================== -->
<div class="modal fade" id="prerequisiteModal" tabindex="-1">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div class="modal-header bg-info text-dark">
                <h5 class="modal-title fw-bold"><i class="fas fa-sitemap"></i> Điều kiện Tiên Quyết: <span id="prereqTitle"></span></h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body p-4">
                <input type="hidden" id="prereqMaMon" />
                
                <div class="d-flex gap-2 mb-4">
                    <select id="prereqSelect" class="form-select border-info">
                        <option value="">-- Chọn môn tiên quyết để thêm --</option>
                    </select>
                    <button class="btn btn-primary fw-bold px-4" type="button" onclick="addPrereq()">
                        <i class="fas fa-plus"></i> Thêm
                    </button>
                </div>

                <div class="table-responsive">
                    <table class="table table-bordered mb-0">
                        <thead class="table-light text-center">
                            <tr>
                                <th>Mã Môn TQ</th>
                                <th>Tên Môn Học</th>
                                <th width="15%">Thao tác</th>
                            </tr>
                        </thead>
                        <tbody id="prereqList">
                            <!-- Dữ liệu load bằng AJAX -->
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>
</div>

<script>
    let currentAllSubjects = [];

    function openPrereqModal(maMon, tenMon) {
        document.getElementById('prereqMaMon').value = maMon;
        document.getElementById('prereqTitle').innerText = maMon + ' - ' + tenMon;
        
        loadPrerequisites(maMon);
        
        var modal = new bootstrap.Modal(document.getElementById('prerequisiteModal'));
        modal.show();
    }

    function loadPrerequisites(maMon) {
        fetch('@Url.Action("GetMonTienQuyet", "Staff")?maMon=' + maMon)
            .then(res => res.json())
            .then(res => {
                if(res.success) {
                    currentAllSubjects = res.all;
                    renderPrereqTable(res.data);
                    renderPrereqSelect(maMon, res.data);
                }
            });
    }

    function renderPrereqTable(data) {
        const tbody = document.getElementById('prereqList');
        tbody.innerHTML = '';
        if(data.length === 0) {
            tbody.innerHTML = '<tr><td colspan="3" class="text-center text-muted py-3">Môn học này không yêu cầu tiên quyết!</td></tr>';
            return;
        }

        data.forEach(item => {
            tbody.innerHTML += 
                <tr>
                    <td class="text-center fw-bold text-primary"></td>
                    <td class="fw-bold"></td>
                    <td class="text-center">
                        <button class="btn btn-sm btn-danger" onclick="removePrereq('')">
                            <i class="fas fa-trash"></i> Xóa
                        </button>
                    </td>
                </tr>
            ;
        });
    }

    function renderPrereqSelect(currentMaMon, existingPrereqs) {
        const select = document.getElementById('prereqSelect');
        select.innerHTML = '<option value="">-- Chọn môn tiên quyết để thêm --</option>';
        
        const existingIds = existingPrereqs.map(x => x.MaMon);
        
        currentAllSubjects.forEach(subject => {
            // Không được chọn chính nó, và không chọn những môn đã thêm
            if(subject.MaMon !== currentMaMon && !existingIds.includes(subject.MaMon)) {
                select.innerHTML += <option value=""> - </option>;
            }
        });
    }

    function addPrereq() {
        const maMon = document.getElementById('prereqMaMon').value;
        const maMonTQ = document.getElementById('prereqSelect').value;
        
        if(!maMonTQ) {
            alert('Vui lòng chọn môn học!');
            return;
        }

        fetch('@Url.Action("AddMonTienQuyet", "Staff")', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ maMon: maMon, maMonTQ: maMonTQ })
        })
        .then(res => res.json())
        .then(res => {
            if(res.success) {
                loadPrerequisites(maMon);
            } else {
                alert(res.message);
            }
        });
    }

    function removePrereq(maMonTQ) {
        if(!confirm('Bạn có chắc chắn muốn xóa điều kiện tiên quyết này?')) return;
        
        const maMon = document.getElementById('prereqMaMon').value;

        fetch('@Url.Action("RemoveMonTienQuyet", "Staff")', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ maMon: maMon, maMonTQ: maMonTQ })
        })
        .then(res => res.json())
        .then(res => {
            if(res.success) {
                loadPrerequisites(maMon);
            } else {
                alert(res.message);
            }
        });
    }
</script>
'''

if 'prerequisiteModal' not in text:
    text = text + prereq_modal
    with codecs.open('Views/Staff/ManageMonHoc.cshtml', 'w', 'utf-8-sig') as f:
        f.write(text)
    print("Added Prerequisite UI to ManageMonHoc.cshtml")
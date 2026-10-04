import codecs

with codecs.open('Views/Student/XemHocPhi.cshtml', 'r', 'utf-8-sig') as f:
    text = f.read()

# Fix Images
text = text.replace('https://vnpay.vn/s1/vnpay/logo.svg', 'https://cdn.haitrieu.com/wp-content/uploads/2022/10/Logo-VNPAY-QR-1.png')
text = text.replace('https://upload.wikimedia.org/wikipedia/vi/f/fe/MoMo_Logo.png', 'https://cdn.haitrieu.com/wp-content/uploads/2022/10/Logo-MoMo-Circle.png')
text = text.replace('https://upload.wikimedia.org/wikipedia/commons/thumb/5/5e/Visa_Inc._logo.svg/2560px-Visa_Inc._logo.svg.png', 'https://upload.wikimedia.org/wikipedia/commons/4/41/Visa_Logo.png')
text = text.replace('https://upload.wikimedia.org/wikipedia/commons/thumb/2/2a/Mastercard-logo.svg/1280px-Mastercard-logo.svg.png', 'https://upload.wikimedia.org/wikipedia/commons/a/a4/Mastercard_2019_logo.svg')

# Add Checkboxes to Table Header
old_thead = '''                            <thead>
                                <tr>
                                    <th class="ps-4 py-3">Mã Lớp HP</th>'''
new_thead = '''                            <thead>
                                <tr>
                                    <th class="ps-4 py-3" width="5%">
                                        <input type="checkbox" id="selectAll" class="form-check-input border-primary" style="transform: scale(1.2);" />
                                    </th>
                                    <th class="py-3">Mã Lớp HP</th>'''
text = text.replace(old_thead, new_thead)

# Add Checkboxes to Table Body
old_tbody = '''                                        <tr>
                                            <td class="ps-4 fw-bold text-primary">@item.MaLopHP</td>'''
new_tbody = '''                                        <tr>
                                            <td class="ps-4">
                                                <input type="checkbox" class="form-check-input border-secondary subject-checkbox" 
                                                       value="@item.MaLopHP" data-amount="@item.ThanhTien" style="transform: scale(1.2);" />
                                            </td>
                                            <td class="fw-bold text-primary">@item.MaLopHP</td>'''
text = text.replace(old_tbody, new_tbody)

# Add Bottom Payment Bar and JS
old_bottom = '''                            </tbody>
                        </table>
                    </div>
                </div>
            </div>'''
            
new_bottom = old_bottom + '''
            <!-- THANH TOÁN HÀNG LOẠT -->
            <div id="batchPayContainer" class="card border-0 shadow mt-4 p-4 d-none" style="background: linear-gradient(to right, #ffffff, #f8f9fa); border-left: 5px solid #0d6efd !important;">
                <div class="d-flex align-items-center justify-content-between">
                    <div>
                        <h5 class="fw-bold mb-1 text-primary">Thanh toán theo lô</h5>
                        <p class="mb-0 text-muted">Bạn đã chọn <strong id="selectedCount" class="text-dark">0</strong> học phần</p>
                    </div>
                    <div class="text-end">
                        <h4 class="fw-bold text-danger mb-2" id="selectedTotal">0 <small class="text-muted fs-6">VNĐ</small></h4>
                        <button type="button" class="btn btn-primary fw-bold px-4 rounded-pill shadow-sm" onclick="processBatchPayment()">
                            <i class="fas fa-credit-card me-2"></i>Thanh toán ngay
                        </button>
                    </div>
                </div>
            </div>
            
            <script>
                document.addEventListener('DOMContentLoaded', function() {
                    const selectAll = document.getElementById('selectAll');
                    const checkboxes = document.querySelectorAll('.subject-checkbox');
                    const batchContainer = document.getElementById('batchPayContainer');
                    const selectedCountLabel = document.getElementById('selectedCount');
                    const selectedTotalLabel = document.getElementById('selectedTotal');
                    
                    function updateBatchUI() {
                        let count = 0;
                        let total = 0;
                        checkboxes.forEach(cb => {
                            if (cb.checked) {
                                count++;
                                total += parseFloat(cb.getAttribute('data-amount'));
                            }
                        });
                        
                        selectedCountLabel.innerText = count;
                        selectedTotalLabel.innerHTML = total.toLocaleString('vi-VN') + ' <small class="text-muted fs-6">VNĐ</small>';
                        
                        if (count > 0) {
                            batchContainer.classList.remove('d-none');
                        } else {
                            batchContainer.classList.add('d-none');
                            selectAll.checked = false;
                        }
                    }

                    if(selectAll) {
                        selectAll.addEventListener('change', function() {
                            checkboxes.forEach(cb => cb.checked = this.checked);
                            updateBatchUI();
                        });
                    }
                    
                    checkboxes.forEach(cb => {
                        cb.addEventListener('change', updateBatchUI);
                    });
                });

                function processBatchPayment() {
                    const checkboxes = document.querySelectorAll('.subject-checkbox:checked');
                    if (checkboxes.length === 0) return;
                    
                    let maLopHPs = [];
                    let totalAmount = 0;
                    
                    checkboxes.forEach(cb => {
                        maLopHPs.push(cb.value);
                        totalAmount += parseFloat(cb.getAttribute('data-amount'));
                    });
                    
                    const maLopHPString = maLopHPs.join(',');
                    
                    // Redirect to ThanhToan action
                    window.location.href = '@Url.Action("ThanhToan", "Student")' + '?maLopHP=' + encodeURIComponent(maLopHPString) + '&tenMon=Batch&soTien=' + totalAmount;
                }
            </script>
'''

text = text.replace(old_bottom, new_bottom)

with codecs.open('Views/Student/XemHocPhi.cshtml', 'w', 'utf-8-sig') as f:
    f.write(text)
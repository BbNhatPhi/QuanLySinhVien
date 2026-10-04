import codecs

with codecs.open('Views/Student/ThanhToan.cshtml', 'r', 'utf-8-sig') as f:
    text = f.read()

text = text.replace('https://vnpay.vn/s1/vnpay/logo.svg', 'https://cdn.haitrieu.com/wp-content/uploads/2022/10/Logo-VNPAY-QR-1.png')
text = text.replace('https://upload.wikimedia.org/wikipedia/commons/thumb/5/5e/Visa_Inc._logo.svg/2560px-Visa_Inc._logo.svg.png', 'https://upload.wikimedia.org/wikipedia/commons/4/41/Visa_Logo.png')
text = text.replace('https://upload.wikimedia.org/wikipedia/commons/thumb/2/2a/Mastercard-logo.svg/1280px-Mastercard-logo.svg.png', 'https://upload.wikimedia.org/wikipedia/commons/a/a4/Mastercard_2019_logo.svg')

with codecs.open('Views/Student/ThanhToan.cshtml', 'w', 'utf-8-sig') as f:
    f.write(text)
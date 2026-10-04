import codecs

with codecs.open('Views/Account/Login.cshtml', 'r', 'utf-8-sig') as f:
    text = f.read()

old_login_btn = '<button type="submit" class="btn btn-primary w-100 py-3 fw-bold login-btn">'
new_login_btn = '''<div class="text-end mb-3">
                                    <a href="@Url.Action("ForgotPassword", "Account")" class="text-decoration-none small fw-bold text-primary">
                                        <i class="fas fa-lock me-1"></i>Quên mật khẩu?
                                    </a>
                                </div>
                                <button type="submit" class="btn btn-primary w-100 py-3 fw-bold login-btn">'''

if 'ForgotPassword' not in text:
    text = text.replace(old_login_btn, new_login_btn)
    with codecs.open('Views/Account/Login.cshtml', 'w', 'utf-8-sig') as f:
        f.write(text)
    print("Added Forgot Password link to Login.cshtml")
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace StudentManagementSystem
{
    public class MvcApplication : System.Web.HttpApplication
    {
        // 1. Hàm khởi chạy ứng dụng (Đã gộp đầy đủ cấu hình gốc của bạn)
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        // 2. Hàm đọc Cookie và cấp quyền (Bắt buộc phải nằm gọn trong class này)
        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null && !string.IsNullOrEmpty(authCookie.Value))
            {
                try
                {
                    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                    if (authTicket != null && !authTicket.Expired)
                    {
                        string[] roles = authTicket.UserData.Split(',');
                        HttpContext.Current.User = new GenericPrincipal(new GenericIdentity(authTicket.Name), roles);
                    }
                }
                catch
                {
                    // Xử lý an toàn nếu cookie cũ bị hỏng
                }
            }
        }
    }
}
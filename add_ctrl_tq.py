import codecs
import re

with codecs.open('Controllers/StaffController.cs', 'r', 'utf-8-sig') as f:
    text = f.read()

new_actions = '''
        // ==========================================
        // QUẢN LÝ MÔN TIÊN QUYẾT (AJAX)
        // ==========================================
        [HttpGet]
        public JsonResult GetMonTienQuyet(string maMon)
        {
            var data = _staffDAO.GetMonTienQuyet(maMon);
            var allMonHoc = _staffDAO.GetAllMonHoc();
            return Json(new { success = true, data = data, all = allMonHoc }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddMonTienQuyet(string maMon, string maMonTQ)
        {
            if(maMon == maMonTQ) return Json(new { success = false, message = "Môn tiên quyết không thể là chính nó!" });
            string result = _staffDAO.AddMonTienQuyet(maMon, maMonTQ);
            if (result == "Success")
            {
                StudentManagementSystem.Helpers.LogHelper.Log($"Giáo vụ thêm môn tiên quyết {maMonTQ} cho môn {maMon}");
                return Json(new { success = true });
            }
            return Json(new { success = false, message = result });
        }

        [HttpPost]
        public JsonResult RemoveMonTienQuyet(string maMon, string maMonTQ)
        {
            string result = _staffDAO.RemoveMonTienQuyet(maMon, maMonTQ);
            if (result == "Success")
            {
                StudentManagementSystem.Helpers.LogHelper.Log($"Giáo vụ xóa môn tiên quyết {maMonTQ} khỏi môn {maMon}");
                return Json(new { success = true });
            }
            return Json(new { success = false, message = result });
        }
'''

if 'GetMonTienQuyet' not in text:
    if 'using StudentManagementSystem.Helpers;' not in text:
        text = text.replace('using System.Web.Mvc;', 'using System.Web.Mvc;\nusing StudentManagementSystem.Helpers;')

    idx = text.rfind('}')
    idx = text.rfind('}', 0, idx)
    text = text[:idx] + new_actions + text[idx:]
    with codecs.open('Controllers/StaffController.cs', 'w', 'utf-8-sig') as f:
        f.write(text)
    print("Added Prerequisite AJAX actions to StaffController")
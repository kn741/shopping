using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using X.PagedList;

namespace shopping.Areas.User.Controllers
{    public class MemberController : Controller
    {
        private readonly dbEntities db;
        private readonly IConfiguration Configuration;
       /// <summary>
        /// 控制器建構子
        /// </summary>
        /// <param name="configuration">環境設定物件</param>
        /// <param name="entities">EF資料管理物件</param>
        public MemberController(IConfiguration configuration, dbEntities entities)
        {
            db = entities;
            Configuration = configuration;
        }
        /// <summary>
        /// init
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Area("User")]
        [Login(RoleList = "User,Mis")]
        public IActionResult Init()
        {
            //初始化Session
            SessionService.SearchText = "";
            SessionService.SortColumn = "";
            SessionService.SortDirection = "";
            return RedirectToAction("Index", ActionService.Controller, new { area = ActionService.Area });
        }

        /// <summary>
        /// 會員列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Area("User")]
        [Login(RoleList = "User,Mis")]
        public IActionResult Index(int page = 1, int pageSize = 5)
        {
            using var user = new z_sqlUsers();
            var model = user.GetRoleUserList("Member").ToPagedList(page, pageSize);
            ViewBag.PageInfo = $"第 {page} 頁，共 {model.PageCount}頁";
            ViewBag.SearchText = SessionService.SearchText;
            SessionService.SetProgramInfo("", "會員資料維護");
            ActionService.SetActionName(enAction.Index);
            return View(model);
        }

        /// <summary>
        /// 會員新增/修改(GET)
        /// </summary>
        /// <param name="id">會員id</param>
        /// <returns></returns>
        [HttpGet]
        [Area("User")]
        [Login(RoleList = "User,Mis")]
        public IActionResult CreateEdit(int id=0)
        {
            using var user = new z_sqlUsers();
            var model = new Users();
            if (id == 0){
                //新增時的預設值
                model.Id=0;
                model.RoleNo="Member";
                model.GenderCode="M";
                model.Birthday=DateTime.Today;
                SessionService.SetProgramInfo("", "新增會員資料");
            }
            else{
                model = user.GetData(id);
                SessionService.SetProgramInfo("", "修改會員資料");
            }
            
            return View(model);
        }

        /// <summary>
        /// 會員新增/修改(POST)
        /// </summary>
        /// <param name="model">會員資料</param>
        /// <returns></returns>
        [HttpPost]
        [Area("User")]
        [Login(RoleList = "User,Mis")]
        public IActionResult CreateEdit(Users model)
        {
            if(!ModelState.IsValid)return View(model);
            var user = new z_sqlUsers();
            if(!user.CheckCreateEditValidation(model)){
                ModelState.AddModelError("UserNo", "會員帳號或電子信箱重覆建檔!");
                // TempData["ErrorMessage"] = $"{model.UserNo},{model.ContactEmail}會員帳號或電子信箱重覆建檔!";
                return View(model);
            }
            user.CreateEdit(model,model.Id);

            return RedirectToAction("Index", ActionService.Controller, new { area = ActionService.Area });
        }

        /// <summary>
        /// 會員刪除
        /// </summary>
        /// <param name="id">會員ID</param>
        /// <returns></returns>
        [HttpPost]
        [Area("User")]
        [Login(RoleList = "User,Mis")]
        public JsonResult Delete(int id = 0)
        {
            using var user = new z_sqlUsers();
            var model = user.GetData(id);   
            user.Delete(id);
            var result = new dmJsonMessage();
            result.Mode = true;
            result.Message = $"{model.UserNo} {model.UserName} 會員已成功刪除!";
            return Json(result);
        }
        /// <summary>
        /// 查詢
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Area("User")]
        [Login(RoleList = "User,Mis")]
        public IActionResult Search()
        {
            object obj_text = Request.Form["SearchText"];
            SessionService.SearchText = (obj_text == null) ? string.Empty : obj_text.ToString();
            return RedirectToAction("Index", ActionService.Controller, new { area = "User" });
        }

        /// <summary>
        /// 欄位排序
        /// </summary>
        /// <param name="id">指定排序的欄位</param>
        /// <returns></returns>
        [HttpGet]
        [Area("User")]
        [Login(RoleList = "User,Mis")]
        public IActionResult Sort(string id)
        {
            if (SessionService.SortColumn == id) 
            {
                SessionService.SortDirection = (SessionService.SortDirection == "asc") ? "desc" : "asc";
            }
            else
            {
                SessionService.SortColumn = id;
                SessionService.SortDirection = "asc";
            }
            return RedirectToAction("Index", ActionService.Controller, new { area = ActionService.Area });
        }

        [HttpGet]
        [Area("User")]
        [Login(RoleList = "User,Mis")]
        public IActionResult ResetPassword(int id)
        {
            using var user = new z_sqlUsers();
            var model = new vmResetPassword();
            model.OldPassword = "super";
            model.NewPassword = Guid.NewGuid().ToString().Replace("-", "").ToUpper().Substring(0, 10);
            string str_code = user.ResetPassword(model);
            user.RegisterConfirm(str_code);
            string str_message = user.CheckMailValidateCode(str_code);

            
            if (string.IsNullOrEmpty(str_message))
            {
                TempData["ErrorMessage"] = "查無會員資料!!";
            }
            else{
                //寄出重設密碼驗證信
                using var sendEmail = new SendMailService();
                var userData = user.GetData(id);
                var mailObject = new MailObject();
                mailObject.MailTime = DateTime.Now;
                mailObject.UserNo = userData.UserNo;
                mailObject.UserName = userData.UserName;
                mailObject.ToName = userData.UserName;
                mailObject.ToEmail = userData.ContactEmail;
                mailObject.Password = userData.Password;
                mailObject.ReturnUrl = "";

                str_message = sendEmail.MemberResetPassword(mailObject);
                if (string.IsNullOrEmpty(str_message))
                {
                    TempData["SuccessMessage"] = "會員重設密碼的要求已受理，請記得提醒會員收信完成重設密碼的流程!!!";
                }
                else{
                    TempData["ErrorMessage"] = str_message;
                }
            }
            return RedirectToAction("Index", ActionService.Controller, new { area = ActionService.Area });
        }

    }
}
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

    }
}
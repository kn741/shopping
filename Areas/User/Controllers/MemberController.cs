using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        /// 會員未結訂單列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Area("User")]
        [Login(RoleList = "User,Mis")]
        public IActionResult Index(int page = 1, int pageSize = 5)
        {
            using var order = new z_sqlUsers();
            var model = order.GetRoleUserList("Member").ToPagedList(page, pageSize);
            ViewBag.PageInfo = $"第 {page} 頁，共 {model.PageCount}頁";
            ViewBag.SearchText = SessionService.SearchText;
            SessionService.SetProgramInfo("", "會員資料維護");
            ActionService.SetActionName(enAction.Index);
            return View(model);
        } 
    }
}
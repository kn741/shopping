using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using X.PagedList;

namespace shopping.Areas.User.Controllers
{
    public class OrderQueryController : Controller
    {
        private readonly dbEntities db;
        private readonly IConfiguration Configuration;

        /// <summary>
        /// 控制器建構子
        /// </summary>
        /// <param name="configuration">環境設定物件</param>
        /// <param name="entities">EF資料管理物件</param>
        public OrderQueryController(IConfiguration configuration, dbEntities entities)
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
            using var order = new z_sqlOrders();
            var model = order.GetALLOrderList(true).ToPagedList(page, pageSize);
            ViewBag.PageInfo = $"第 {page} 頁，共 {model.PageCount}頁";
            ViewBag.SearchText = SessionService.SearchText;
            SessionService.SetProgramInfo("", "已結訂單");
            ActionService.SetActionName(enAction.Index);
            return View(model);
        }
        /// <summary>
        /// 會員訂單列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Area("User")]
        [Login(RoleList = "User,Mis")]
        public IActionResult OrderAll()
        {
            using var order = new z_sqlOrders();
            var model = order.GetALLOrderList();
            SessionService.SetProgramInfo("", "所有訂單");
            ActionService.SetActionName(enAction.Index);
            return View(model);
        }

        /// <summary>
        /// 會員訂單明細
        /// </summary>
        /// <param name="id">訂單表頭ID</param>
        /// <returns></returns>
        [HttpGet]
        [Area("User")]
        [Login(RoleList = "User,Mis")]
        public IActionResult Detail(int id = 0)
        {
            var model = new vmOrderDetail();
            var order = new z_sqlOrders();
            var detail = new z_sqlOrderDetails();
            model.Master = order.GetOrder(id);
            model.Details = detail.GetOrderDetails(id);
            SessionService.SetProgramInfo("", "訂單明細");
            ActionService.SetActionName(enAction.Detail);
            return View(model);
        }
        /// <summary>
        /// 會員訂單明細
        /// </summary>
        /// <param name="id">訂單表頭ID</param>
        /// <returns></returns>
        [HttpGet]
        [Area("User")]
        [Login(RoleList = "User,Mis")]
        public IActionResult Status(int id = 0)
        {
            using var order = new z_sqlOrders();
            var model = order.GetOrder(id);
            SessionService.SetProgramInfo("", "訂單狀態變更");
            ActionService.SetActionName(enAction.Edit);
            return View(model);
        }

        /// <summary>
        /// 會員訂單明細
        /// </summary>
        /// <param name="model">訂單狀態模型</param>
        /// <returns></returns>
        [HttpPost]
        [Area("User")]
        [Login(RoleList = "User,Mis")]
        public IActionResult Status(Orders model)
        {
            using var order = new z_sqlOrders();
            order.ChangeStatus(model.Id, model.StatusCode);
            return RedirectToAction("Index", ActionService.Controller, new { area = ActionService.Area });
        }
        /// <summary>
        /// 取消訂單
        /// </summary>
        /// <param name="id">訂單Id</param>
        /// <returns></returns>
        [HttpGet]
        [Area("User")]
        [Login(RoleList = "User,Mis")]
        public IActionResult Cancel(int id = 0)
        {
            using var order = new z_sqlOrders();
            order.CancelOrder(id);
            return RedirectToAction("Index", ActionService.Controller, new { area = ActionService.Area });
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
            return RedirectToAction("Index", "Order", new { area = "User" });
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
    }
}
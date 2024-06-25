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


        public OrderQueryController(IConfiguration configuration, dbEntities entities){
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
        public IActionResult Init(){

            return RedirectToAction("Index", "Order", new { area = "User" });
        }
        
        /// <summary>
        /// 會員未結訂單列表
        /// </summary>
        /// <param name="page">目前頁數</param>
        /// <param name="pageSize">每頁筆數</param>
        /// <returns></returns>
        [HttpGet]
        [Area("User")]
        [Login(RoleList = "User,Mis")]
        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            using var order = new z_sqlOrders();
            var model = order.GetALLOrderList(false).ToPagedList(page, pageSize);
            ViewBag.PageInfo = $"第 {page} 頁，共 {model.PageCount}頁";
            SessionService.SetProgramInfo("", "未結訂單");
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
        /// 會員訂單狀態變更
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
        /// 會員訂單狀態變更
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
            return RedirectToAction("Index", "Order", new { area = "User" });
        }
        /// <summary>
        /// 會員訂單退貨
        /// </summary>
        /// <param name="id">訂單Id</param>
        /// <returns></returns>
        [HttpGet]
        [Area("User")]
        [Login(RoleList = "User,Mis")]
        public IActionResult Return(int id=0)
        {
            using var order = new z_sqlOrders();
            order.ReturnOrder(id);
            return RedirectToAction("Index", "Order", new { area = "User" });
        }
    }
}
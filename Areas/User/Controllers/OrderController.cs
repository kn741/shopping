using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace shopping.Areas.User.Controllers
{
    public class OrderController : Controller
    {
        private readonly dbEntities db;
        private readonly IConfiguration Configuration;

        /// <summary>
        /// 控制器建構子
        /// </summary>
        /// <param name="configuration">環境設定物件</param>
        /// <param name="entities">EF資料管理物件</param>
        public OrderController(IConfiguration configuration, dbEntities entities){
            db = entities;
            Configuration = configuration;
        }
        /// <summary>
        /// 
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
        /// <returns></returns>
        [HttpGet]
        [Area("User")]
        [Login(RoleList = "User,Mis")]
        public IActionResult Index()
        {
            using var order = new z_sqlOrders();
            var model = order.GetALLOrderList(false);
            SessionService.SetProgramInfo("", "未結訂單");
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
            return RedirectToAction("Index", "Order", new { area = "User" });
        }

    }
}
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
            return View(model);
        }

    }
}
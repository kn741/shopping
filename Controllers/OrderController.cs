using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace shopping.Controllers
{
    public class OrderController : Controller
    {
        [Login(RoleList = "Member")]
        [HttpGet]
        public IActionResult Index(string id="unclose")
        {
            bool bln_isClosed = true;
            using var order = new z_sqlOrders();
            var model = new List<Orders>();
            if (id == "unclose") bln_isClosed = false;
            model = order.GetOrderList(bln_isClosed);
            
            //id代號
            SessionService.StringValue1 = id;
            
            string str_action = bln_isClosed ? "已結訂單查詢" : "未結訂單查詢";
            SessionService.SetProgramInfo("", str_action);
            ActionService.SetActionName(enAction.Index);
            return View(model);
        }

        [Login(RoleList = "Member")]
        [HttpGet]
        public IActionResult Detail(int id = 0)
        {
            var model = new vmOrderDetail();
            var order = new z_sqlOrders();
            var detail = new z_sqlOrderDetails();
            model.Master = order.GetOrder(id);
            model.Details = detail.GetOrderDetails();
            SessionService.SetProgramInfo("", "訂單明細");
            ActionService.SetActionName(enAction.Detail);
            return View(model);
        }
    }
}
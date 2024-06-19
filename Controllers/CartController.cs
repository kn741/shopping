using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace shopping.Controllers
{
    public class CartController : Controller
    {
        /// <summary>
        /// 購物車列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            using var cart = new z_sqlCarts();
            var model = cart.GetDataList();
            return View(model);
        }

        /// <summary>
        /// 加入購物車
        /// </summary>
        /// <param name="id">商品編號</param>
        /// <param name="prodSpec">商品規格</param>
        /// <param name="qty">數量</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult AddCart(string id, string prodSpec = "",int qty = 1)
        {
            using var cart = new z_sqlCarts();
            cart.AddCart(id, prodSpec, qty);
            return RedirectToAction("Index", "Cart", new { area = "" });
        }

        /// <summary>
        /// 立即結帳
        /// </summary>
        /// <param name="id">商品編號</param>
        /// <param name="qty">數量</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Checkout(string id, int qty = 1)
        {
            using var cart = new z_sqlCarts();
            cart.AddCart(id, "", qty);
            return RedirectToAction("Payment", "Cart", new { area = "" });
        }

        /// <summary>
        /// 更新購物車
        /// </summary>
        /// <param name="id">商品編號</param>
        /// <param name="qty">數量</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult UpdateCart(string id, int qty)
        {
            using var cart = new z_sqlCarts();
            cart.UpdateCart(id, qty);
            return RedirectToAction("Index", "Cart", new { area = "" });
        }

        /// <summary>
        /// 更新購物車數量
        /// </summary>
        /// <param name="prodNo">商品編號</param>
        /// <param name="qty">數量</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult UpdateQty(string prodNo, int qty)
        {
            using var cart = new z_sqlCarts();
            cart.UpdateCart(prodNo, qty);
            var CartTotal = cart.GetCartTotals();
            return Json(new { success = true, value = CartTotal });
        }

        /// <summary>
        /// 刪除購物車
        /// </summary>
        /// <param name="id">購物車Id</param>
        [HttpGet]
        public IActionResult DeleteCart(int id)
        {
            using var cart = new z_sqlCarts();
            cart.DeleteCart(id);
            return RedirectToAction("Index", "Cart", new { area = "" });
        }

        /// <summary>
        /// 刪除購物車(複數)
        /// </summary>
        /// <param name="id">購物車Id</param>
        [HttpGet]
        public IActionResult DeleteMutipleCart(int[] id)
        {
            using var cart = new z_sqlCarts();
            foreach (int item in id) { cart.DeleteCart(item); }
            return RedirectToAction("Index", "Cart", new { area = "" });
        }

        /// <summary>
        /// 加入購物車
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddCart()
        {
            string str_prod_spec = "";
            object obj_text = Request.Form["qtybutton"];
            string str_qty = (obj_text == null) ? "1" : obj_text.ToString();
            int int_qty = int.Parse(str_qty);
            obj_text = Request.Form["prodNo"];
            string str_prod_no = (obj_text == null) ? string.Empty : obj_text.ToString();
            using var prodProperty = new z_sqlProductPropertys();
            List<Propertys> PropertyList = prodProperty.GetProductPropertyList(str_prod_no);

            foreach (var item in PropertyList){
                obj_text = Request.Form[item.PropertyNo];
                if(obj_text!= null){
                    string str_prop_value = obj_text.ToString();
                    if(!string.IsNullOrEmpty(str_prod_spec))str_prod_spec+=",";
                    str_prod_spec += item.PropertyName + ":" + str_prop_value;
                }
            }

            return RedirectToAction("AddCart", "Cart", new { area = "", id = str_prod_no , prodSpec = str_prod_spec, qty = int_qty });
        }
        /// <summary>
        /// Payment頁面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Login(RoleList = "Member,User")]
        public IActionResult Payment()
        {
            using var users = new z_sqlUsers();
            var model = users.GetPaymentUser();
            return View(model);
        }

        /// <summary>
        /// Payment頁面(Post)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Login(RoleList = "Member,User")]
        public IActionResult Payment(vmOrders model)
        {
            if (string.IsNullOrEmpty(model.ReceiveName)) model.ReceiveName = model.MemberName;
            if (string.IsNullOrEmpty(model.ReceiveEmail)) model.ReceiveEmail = model.MemberEmail;
            if (string.IsNullOrEmpty(model.ReceiveAddress)) model.ReceiveAddress = model.MemberAddress;
            if (string.IsNullOrEmpty(model.ReceiveTel)) model.ReceiveTel = model.MemberTel;
            PaymentService.SetPaymentData(model);
            return RedirectToAction("PaymentConfirm", "Cart", new { area = "" });
        }

        [HttpGet]
        [Login(RoleList = "Member,User")]
        public IActionResult PaymentConfirm()
        {
            var model = PaymentService.GetPaymentData();
            return View(model);
        }
        [HttpPost]
        [Login(RoleList = "Member,User")]
        public IActionResult PaymentConfirm(vmOrders model)
        {
            //產生訂單
            var data = PaymentService.GetPaymentData();
            data.PaymentNo = model.PaymentNo;
            data.ShippingNo = model.ShippingNo;
            string str_order_No = PaymentService.CreateOrder(data);

            var carts = new z_sqlCarts();
            //顯示訂單已完成
            SessionService.MessageText = $"您的訂單已建立，訂單編號為：{str_order_No}，請注意到貨訊息!";
            SessionService.StringValue1 = "ShopIndex";
            
            
            //清除購物車
            carts.DeleteCart();
            
            return RedirectToAction("Index", "Message", new { area = "" });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PagedList.Core;

namespace shopping.Models
{
    public class z_sqlCarts : DapperSql<Carts>
    {
        public z_sqlCarts()
        {
            OrderByColumn = SessionService.SortColumn;
            OrderByDirection = SessionService.SortDirection;
            DefaultOrderByColumn = "Carts.ProdNo";
            DefaultOrderByDirection = "ASC";
            if (string.IsNullOrEmpty(OrderByColumn)) OrderByColumn = DefaultOrderByColumn;
            if (string.IsNullOrEmpty(OrderByDirection)) OrderByDirection = DefaultOrderByDirection;
        }

        public override string GetSQLSelect()
        {
            string str_query = @"
SELECT Carts.Id, Carts.LotNo, Carts.MemberNo, Users.UserName AS MemberName, Carts.VendorNo, Carts.CategoryNo, 
Carts.CategoryName, Carts.ProdNo, Carts.ProdName, Carts.ProdSpec, Carts.OrderQty, Carts.OrderPrice, 
Carts.OrderAmount, Carts.CreateTime, Carts.Remark
FROM Carts 
LEFT OUTER JOIN Users ON Carts.MemberNo = Users.UserNo
";
            return str_query;
        }

        public override List<string> GetSearchColumns()
        {
            List<string> searchColumn = dpr.GetStringColumnList(EntityObject);
            searchColumn.Add("Users.UserName");
            return searchColumn;
        }

        /// <summary>
        /// 取得目前購物車明細
        /// </summary>
        /// <returns></returns>
        public override List<Carts> GetDataList()
        {
            string str_query = GetSQLSelect();
            str_query += " WHERE Carts.LotNo = @LotNo";
            DynamicParameters parm = new DynamicParameters();
            parm.Add("LotNo", CartService.LotNo);
            var model = dpr.ReadAll<Carts>(str_query, parm);
            return model;
        }

        /// <summary>
        /// 遊客購物車合併至會員購物車
        /// </summary>
        public void MergeCart()
        {
            //取得遊客購物車明細
            var data = GetDataList();

            //更新購物車批號
            CartService.NewLotNo();

            //將新批號寫入會員購物車
            string str_query = "UPDATE Carts SET LotNo = @LotNo WHERE MemberNo = @MemberNo";
            DynamicParameters parm = new DynamicParameters();
            parm.Add("LotNo", CartService.LotNo);
            parm.Add("MemberNo", SessionService.UserNo);
            dpr.Execute(str_query, parm);

            //將遊客購物車合併至會員購物車
            foreach (var item in data)
            {
                AddCart(item.ProdNo, item.ProdSpec, item.OrderQty);
            }
        }
        /// <summary>
        /// 加入商品至目前批號購物車中
        /// </summary>
        /// <param name="prodNo">商品編號</param>
        /// <param name="prodSpec">商品規格(空白可自抓)</param>
        /// <param name="qty">數量</param>
        public void AddCart(string prodNo, string prodSpec, int qty)
        {
            string str_query = "";
            using var prod = new z_sqlProducts();
            var prodData = prod.GetData(prodNo);
            if (string.IsNullOrEmpty(prodSpec))
            {
                using (var prodProp = new z_sqlProductPropertys())
                {
                    prodSpec = prodProp.GetProductSpec(prodNo);
                }
            }
            int int_price = (prodData.DiscountPrice != 0) ? prodData.DiscountPrice : prodData.SalePrice;
            int int_qty = qty;
            int int_amount = int_qty * int_price;
            str_query = GetSQLSelect();
            str_query += " WHERE LotNo = @LotNo AND ProdNo = @ProdNo AND ProdSpec = @ProdSpec";
            DynamicParameters parm = new DynamicParameters();
            parm.Add("LotNo", CartService.LotNo);
            parm.Add("ProdNo", prodNo);
            parm.Add("ProdSpec", prodSpec);
            var data = dpr.ReadSingle<Carts>(str_query, parm);
            if (data == null || data.Id == 0)
            {
                str_query = @"
INSERT INTO Carts
(LotNo,MemberNo,VendorNo,CategoryNo,CategoryName
,ProdNo,ProdName,ProdSpec,OrderQty,OrderPrice
,OrderAmount,CreateTime,Remark)
VALUES 
(@LotNo,@MemberNo,@VendorNo,@CategoryNo,@CategoryName
,@ProdNo,@ProdName,@ProdSpec,@OrderQty,@OrderPrice
,@OrderAmount,@CreateTime,@Remark)
";
                parm.Add("MemberNo", SessionService.UserNo);
                parm.Add("VendorNo", "");
                parm.Add("CategoryNo", prodData.CategoryNo);
                parm.Add("CategoryName", prodData.CategoryName);
                parm.Add("ProdName", prodData.ProdName);
                parm.Add("OrderQty", int_qty);
                parm.Add("OrderPrice", int_price);
                parm.Add("OrderAmount", int_amount);
                parm.Add("CreateTime", DateTime.Now);
                parm.Add("Remark", "");
            }
            else
            {
                int_qty += data.OrderQty;
                int_amount = int_qty * int_price;
                str_query = @"
UPDATE Carts SET OrderQty = @OrderQty , OrderAmount = @OrderAmount , CreateTime = @CreateTime 
WHERE LotNo = @LotNo AND ProdNo = @ProdNo AND ProdSpec = @ProdSpec
";
                parm.Add("OrderQty", int_qty);
                parm.Add("OrderAmount", int_amount);
                parm.Add("CreateTime", DateTime.Now);
            }
            dpr.Execute(str_query, parm);
        }
        /// <summary>
        /// 更新購物車
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="qty">數量</param>
        public void UpdateCart(int id, int qty)
        {
            int int_qty = qty;
            int int_price = 0;
            int int_amount = 0;
            string str_query = "";

            str_query = GetSQLSelect();
            str_query += " WHERE Id = @Id";
            DynamicParameters parm = new DynamicParameters();
            parm.Add("Id", id);
            var data = dpr.ReadSingle<Carts>(str_query, parm);
            if (data != null)
            {
                int_qty += data.OrderQty;
                int_price = data.OrderPrice;
                int_amount = int_qty * int_price;

                str_query = @"
UPDATE Carts SET OrderQty = @OrderQty , OrderAmount = @OrderAmount , CreateTime = @CreateTime 
WHERE Id = @Id
";
                parm.Add("OrderQty", qty);
                parm.Add("OrderAmount", int_amount);
                parm.Add("CreateTime", DateTime.Now);
                dpr.Execute(str_query, parm);
            }
        }
        /// <summary>
        /// 更新購物車
        /// </summary>
        /// <param name="prodNo">商品編號</param>
        /// <param name="qty">數量</param>
        public void UpdateCart(string prodNo, int qty)
        {
            string str_query = "";
            DynamicParameters parm = new DynamicParameters();
            str_query = GetSQLSelect();
            str_query += " WHERE LotNo = @LotNo AND ProdNo = @ProdNo";
            parm.Add("LotNo", CartService.LotNo);
            parm.Add("ProdNo", prodNo);
            var data = dpr.ReadSingle<Carts>(str_query, parm);
            if (data != null)
            {
                str_query = @"
UPDATE Carts SET OrderQty = @OrderQty , OrderAmount = @OrderAmount , CreateTime = @CreateTime 
WHERE Id = @Id
";
                parm.Add("Id", data.Id);
                parm.Add("OrderQty", qty);
                parm.Add("OrderAmount", qty * data.OrderPrice);
                parm.Add("CreateTime", DateTime.Now);
                dpr.Execute(str_query, parm);
            }
        }
        /// <summary>
        /// 刪除購物車
        /// </summary>
        /// <param name="id">Id</param>
        public void DeleteCart(int id)
        {
            string str_query = "";
            DynamicParameters parm = new DynamicParameters();
            str_query = "DELETE FROM Carts WHERE Id = @Id";
            parm.Add("Id", id);
            dpr.Execute(str_query, parm);
        }
        /// <summary>
        /// 刪除目前批號購物車
        /// </summary>
        public void DeleteCart()
        {
            string str_query = "";
            DynamicParameters parm = new DynamicParameters();
            str_query = "DELETE FROM Carts WHERE LotNo = @LotNo";
            parm.Add("LotNo", CartService.LotNo);
            dpr.Execute(str_query, parm);
        }
        /// <summary>
        /// 取得目前批號購物車筆數
        /// </summary>
        /// <returns></returns>
        public int GetCartCount()
        {
            List<Carts> data = new List<Carts>();
            data = GetDataList();
            return data.Count();
        }
        /// <summary>
        /// 取得目前批號購物車合計
        /// </summary>
        /// <returns></returns>
        public int GetCartTotal()
        {
            List<Carts> data = new List<Carts>();
            data = GetDataList();
            return data.Sum(m => m.OrderQty * m.OrderPrice);
        }

        public dmCartTotal GetCartTotals()
        {
            var model = new dmCartTotal();
            model.Amount = GetCartTotal();
            model.Freight = (model.Amount > 0) ? 60 : 0;
            model.Total = model.Amount + model.Freight;
            return model;
        }
    }
}
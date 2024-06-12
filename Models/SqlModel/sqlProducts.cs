using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace shopping.Models
{
    public class z_sqlProducts : DapperSql<Products>
    {
        public z_sqlProducts()
        {
            OrderByColumn = SessionService.SortColumn;
            OrderByDirection = SessionService.SortDirection;
            DefaultOrderByColumn = "Products.ProdNo";
            DefaultOrderByDirection = "ASC";
            if (string.IsNullOrEmpty(OrderByColumn)) OrderByColumn = DefaultOrderByColumn;
            if (string.IsNullOrEmpty(OrderByDirection)) OrderByDirection = DefaultOrderByDirection;
        }

        public override string GetSQLSelect()
        {
            string str_query = @"
SELECT Products.Id, Products.ProdNo, Products.ProdName, Products.CategoryNo, Categorys.CategoryName, 
Products.InventoryQty, Products.StatusNo, ProductStatus.StatusName, Products.SpecificationText, Products.ContentText,Products.CostPrice, 
Products.SalePrice,Products.MarketPrice, Products.DiscountPrice, Products.Remark, Products.VendorNo,Products.BrandName,Products.BrandSeriesName,
CASE WHEN Products.DiscountPrice != '0' THEN Products.DiscountPrice ELSE Products.SalePrice END AS MinPrice 
FROM Products 
LEFT OUTER JOIN ProductStatus ON Products.StatusNo = ProductStatus.StatusNo 
LEFT OUTER JOIN Categorys ON Products.CategoryNo = Categorys.CategoryNo 
";
            return str_query;
        }

        public override List<string> GetSearchColumns()
        {
            //由系統自動取得文字欄位的集合
            List<string> searchColumn;
            searchColumn = dpr.GetStringColumnList(EntityObject);
            searchColumn.Add("Categorys.CategoryName");
            searchColumn.Add("ProductStatus.StatusName");
            return searchColumn;
        }

        /// <summary>
        /// 取得單筆資料(同步呼叫)
        /// </summary>
        /// <param name="prodNo">商品代號</param>
        /// <returns></returns>
        public Products GetData(string prodNo)
        {
            var model = new Products();
            using var dpr = new DapperRepository();
            string sql_query = GetSQLSelect();
            string sql_where = "WHERE Products.ProdNo = @ProdNo ";
            sql_query += sql_where;
            sql_query += GetSQLOrderBy();
            DynamicParameters parm = new DynamicParameters();
            if (!string.IsNullOrEmpty(sql_where))
            {
                //自定義的 Where Parm 參數
                parm.Add("ProdNo", prodNo);
            }
            model = dpr.ReadSingle<Products>(sql_query, parm);
            return model;
        }

        /// <summary>
        /// 取得商家商品資料(同步呼叫)
        /// </summary>
        /// <param name="userNo">使用者代號</param>
        /// <returns></returns>
        public List<Products> GetVendorData(string userNo)
        {
            var model = new List<Products>();
            using var dpr = new DapperRepository();
            string sql_query = GetSQLSelect();
            string sql_where = "WHERE Products.VendorNo = @userNo ";
            sql_query += sql_where;
            sql_query += GetSQLOrderBy();
            DynamicParameters parm = new DynamicParameters();
            if (!string.IsNullOrEmpty(sql_where))
            {
                //自定義的 Where Parm 參數
                parm.Add("userNo", userNo);
            }
            model = dpr.ReadAll<Products>(sql_query, parm);
            return model;
        }

        /// <summary>
        /// 取得多筆資料(同步呼叫)
        /// </summary>
        /// <param name="sortNo">排序方式</param>
        /// <param name="searchString">模糊搜尋文字(空白或不傳入表示不搜尋)</param>
        /// <returns></returns>
        public List<Products> GetDataList(string sortNo, string searchString = "")
        {
            List<string> searchColumns = GetSearchColumns();
            DynamicParameters parm = new DynamicParameters();
            var model = new List<Products>();
            using var dpr = new DapperRepository();
            string sql_query = GetSQLSelect();
            string sql_where = GetSQLWhere();
            sql_query += sql_where;
            if (!string.IsNullOrEmpty(searchString))
                sql_query += dpr.GetSQLWhereBySearchColumn(new Products(), searchColumns, sql_where, searchString);
            if (!string.IsNullOrEmpty(sql_where))
            {
                //自定義的 Where Parm 參數
                //parm.Add("參數名稱", "參數值");
            }
            if (sortNo == "High") sql_query += " ORDER BY MinPrice DESC;";
            else if (sortNo == "Low") sql_query += " ORDER BY MinPrice ASC";
            else sql_query += " ORDER BY Products.ProdNo ASC";

            model = dpr.ReadAll<Products>(sql_query, parm);
            return model;
        }


        /// <summary>
        /// 取得多筆資料(同步呼叫)
        /// </summary>
        /// <param name="categoryNo">Category No</param>
        /// <param name="sortNo">排序方式</param>
        /// <param name="searchString">模糊搜尋文字(空白或不傳入表示不搜尋)</param>
        /// <returns></returns>
        public List<Products> GetDataList(string categoryNo, string sortNo, string searchString = "")
        {
            List<string> searchColumns = GetSearchColumns();
            DynamicParameters parm = new DynamicParameters();
            var model = new List<Products>();
            using var dpr = new DapperRepository();
            string sql_query = GetSQLSelect();
            string sql_where = " WHERE Products.CategoryNo = @CategoryNo ";
            sql_query += sql_where;
            if (!string.IsNullOrEmpty(searchString))
                sql_query += dpr.GetSQLWhereBySearchColumn(new Products(), searchColumns, sql_where, searchString);
            if (!string.IsNullOrEmpty(sql_where))
            {
                //自定義的 Where Parm 參數
                parm.Add("CategoryNo", categoryNo);
            }
            if (sortNo == "High") sql_query += " ORDER BY MinPrice DESC;";
            else if (sortNo == "Low") sql_query += " ORDER BY MinPrice ASC";
            else sql_query += " ORDER BY Products.ProdNo ASC";

            model = dpr.ReadAll<Products>(sql_query, parm);
            return model;
        }

        /// <summary>
        /// 取得指定分類商品資料(同步呼叫)
        /// </summary>
        /// <param name="categoryNo">分類代號</param>
        /// <returns></returns>
        public List<Products> GetCategoryDataList(string categoryNo = "All")
        {
            List<string> searchColumns = GetSearchColumns();
            var model = new List<Products>();
            using var dpr = new DapperRepository();
            using var cate = new z_sqlCategorys();
            DynamicParameters parm = new DynamicParameters();
            string sql_query = GetSQLSelect();
            if (!string.IsNullOrEmpty(SessionService.SearchText))
            {
                sql_query += $" WHERE (Products.ProdNo LIKE '%{SessionService.SearchText}%' ";
                sql_query += $" OR Products.ProdName LIKE '%{SessionService.SearchText}%') ";
                sql_query += " AND Products.IsEnabled = @IsEnabled";
                parm.Add("IsEnabled", true);
                model = dpr.ReadAll<Products>(sql_query, parm);
            }
            else
            {
                if (categoryNo != "All")
                {

                    var data = cate.GetData(categoryNo);
                    if (String.IsNullOrEmpty(data.ParentNo))
                    {
                        sql_query += " WHERE Categorys.ParentNo = @ParentNo AND Products.IsEnabled = @IsEnabled";
                        parm.Add("ParentNo", categoryNo);
                        parm.Add("IsEnabled", true);
                    }
                    else
                    {
                        sql_query += " WHERE Products.CategoryNo = @CategoryNo AND Products.IsEnabled = @IsEnabled";
                        parm.Add("CategoryNo", categoryNo);
                        parm.Add("IsEnabled", true);
                    }
                    sql_query += GetSortOrderBy();
                    model = dpr.ReadAll<Products>(sql_query, parm);
                }
                else
                {
                    sql_query += GetSortOrderBy();
                    model = dpr.ReadAll<Products>(sql_query);
                }
            }
            return model;
        }

        private string GetSortOrderBy()
        {
            if (SessionService.SortNo == "High") return " ORDER BY Products.CategoryNo , Products.SalePrice DESC";
            if (SessionService.SortNo == "Low") return " ORDER BY Products.CategoryNo , Products.SalePrice ASC";
            if (SessionService.SortNo == "Product") return " ORDER BY Products.CategoryNo , Products.ProdNo ASC";
            return " ORDER BY Products.ProdNo";
        }

        /// <summary>
        /// 建立商品資訊
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string CreateNewProduct(vmProductCreate model)
        {
            using var dpr = new DapperRepository();
            // using var cryp = new CryptographyService();
            // string str_code = Guid.NewGuid().ToString().Replace("-", "");
            // int id = GetLastID()+1;
            string prodNo = CreateProdID(model.CategoryNo);
            string sql_query = @"
INSERT INTO Products 
(IsEnabled, IsInventory, IsShowPhoto, ProdNo, ProdName,BrandName,BrandSeriesName,BarcodeNo,StatusNo,
VendorNo, CategoryNo, CostPrice, MarketPrice, SalePrice, DiscountPrice,InventoryQty,ContentText,
SpecificationText,Remark)   
VALUES
(@IsEnabled, @IsInventory, @IsShowPhoto, @ProdNo, @ProdName,@BrandName,@BrandSeriesName,@BarcodeNo,@StatusNo,
@VendorNo, @CategoryNo, @CostPrice, @MarketPrice, @SalePrice, @DiscountPrice,@InventoryQty,@ContentText,
@SpecificationText,@Remark)   
";
            DynamicParameters parm = new DynamicParameters();
            parm = dpr.ModelToParm(model, sql_query);
            // parm.Add("Id", id);
            parm.Add("IsEnabled", true);
            parm.Add("IsInventory", false);
            parm.Add("IsShowPhoto", true);
            parm.Add("ProdNo", prodNo);
            parm.Add("ProdName", model.ProdName);
            parm.Add("BrandName", model.BrandName);
            parm.Add("BrandSeriesName", model.BrandSeriesName);
            parm.Add("BarcodeNo", CreateBarcodeNo());
            parm.Add("StatusNo", model.StatusNo);
            parm.Add("VendorNo", SessionService.UserNo);
            parm.Add("CategoryNo",model.CategoryNo);
            parm.Add("MarketPrice", model.MarketPrice);
            parm.Add("SalePrice", model.SalePrice);
            parm.Add("DiscountPrice",model.DiscountPrice);
            parm.Add("InventoryQty", 1);
            parm.Add("ContentText", model.ContentText);
            parm.Add("SpecificationText", model.SpecificationText);
            parm.Add("Remark", "");

            dpr.Execute(sql_query, parm);
            return prodNo;
        }

        // /// <summary>
        // /// 取得最後一筆商品代號(同步呼叫)
        // /// </summary>
        // /// <returns></returns>
        // public int GetLastID()
        // {
        //     using var dpr = new DapperRepository();
        //     string sql_query = "SELECT MAX(Id) As Id FROM Products";
        //     var id = dpr.ReadSingle<Products>(sql_query);
        //     return id.Id;
        // }

        /// <summary>
        /// 建立商品代號(同步呼叫)
        /// </summary>
        /// <param name="gender">商品代號</param>
        /// <returns></returns>
        public string CreateProdID(string gender)
        {
            string genderCode="";
            if(gender.Contains("man")) genderCode = "m";
            else if(gender.Contains("woman")) genderCode = "w";

            using var dpr = new DapperRepository();
            //依性別尋找最後一筆商品代號
            string sql_query = $"SELECT top(1) ProdNo FROM Products where ProdNo like '{genderCode}%' order by CAST(SUBSTRING(ProdNo, 2, LEN(ProdNo) - 1) AS INT) DESC";
            var id = dpr.ReadSingle<Products>(sql_query);
            string numberPart = id.ProdNo.Substring(1);
            int number = int.Parse(numberPart);
            number++;
            genderCode = genderCode+number;
            return genderCode;
        }
        /// <summary>
        /// 建立商品條碼(同步呼叫)
        /// </summary>
        /// <returns></returns>
        public string CreateBarcodeNo()
        {   
            int code = new Random().Next(10000000, 99999999);
            return code+"";
        }
        
        /// <summary>
        ///刪除商品(同步呼叫)
        /// </summary>
        /// <param name="prodNo">商品代號</param>
        /// <returns></returns>
        public void DeleteProduct(string prodNo)
        {
            using var dpr = new DapperRepository();
            string sql_query = $"DELETE FROM Products WHERE ProdNo = '{prodNo}'";
            dpr.Execute(sql_query);
        }

        public void UpdateProduct(Products model)
        {
            using var dpr = new DapperRepository();
            // using var cryp = new CryptographyService();
            // string str_code = Guid.NewGuid().ToString().Replace("-", "");
            // int id = GetLastID()+1;
            string sql_query = @"
UPDATE Products 
SET 
ProdName = @ProdName,
BrandName = @BrandName,
BrandSeriesName = @BrandSeriesName,
StatusNo = @StatusNo,
CategoryNo = @CategoryNo,
CostPrice = @CostPrice,
MarketPrice = @MarketPrice,
SalePrice = @SalePrice,
DiscountPrice = @DiscountPrice,
ContentText = @ContentText,
SpecificationText = @SpecificationText
WHERE ProdNo = @ProdNo
";
            DynamicParameters parm = new DynamicParameters();
            parm = dpr.ModelToParm(model, sql_query);
            // parm.Add("Id", id);
            parm.Add("ProdName", model.ProdName);
            parm.Add("BrandName", model.BrandName);
            parm.Add("BrandSeriesName", model.BrandSeriesName);
            parm.Add("StatusNo", model.StatusNo);
            parm.Add("CategoryNo",model.CategoryNo);
            parm.Add("CostPrice", model.CostPrice);
            parm.Add("MarketPrice", model.MarketPrice);
            parm.Add("SalePrice", model.SalePrice);
            parm.Add("DiscountPrice",model.DiscountPrice);
            parm.Add("ContentText", model.ContentText);
            parm.Add("SpecificationText", model.SpecificationText);
            parm.Add("ProdNo",model.ProdNo);
            dpr.Execute(sql_query, parm);
        }

    }
}
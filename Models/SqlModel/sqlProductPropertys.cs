using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace shopping.Models

{
    public class z_sqlProductPropertys : DapperSql<Departments>
    {
        public z_sqlProductPropertys()
        {
            OrderByColumn = SessionService.SortColumn;
            OrderByDirection = SessionService.SortDirection;
            DefaultOrderByColumn = "ProductPropertys.ProdNo,ProductPropertys.PropertyNo";
            DefaultOrderByDirection = "ASC,ASC";
            if (string.IsNullOrEmpty(OrderByColumn)) OrderByColumn = DefaultOrderByColumn;
            if (string.IsNullOrEmpty(OrderByDirection)) OrderByDirection = DefaultOrderByDirection;
        }
        public override string GetSQLSelect()
        {
            string str_query = @"SELECT ProductPropertys.Id, ProductPropertys.IsSelect, ProductPropertys.ProdNo, Propertys.PropertyName, 
                   Products.ProdName,ProductPropertys.PropertyNo,ProductPropertys.PropertyValue, ProductPropertys.Remark
                   FROM ProductPropertys 
                   LEFT OUTER JOIN Propertys ON ProductPropertys.PropertyNo = Propertys.PropertyNo 
                   LEFT OUTER JOIN Products ON ProductPropertys.ProdNo = Products.ProdNo";
            return str_query;
        }

        public override List<string> GetSearchColumns()
        {
            List<string> searchColumn = dpr.GetStringColumnList(EntityObject);
            searchColumn.Add("Products.ProdName");
            searchColumn.Add("Propertys.PropertysName");
            return searchColumn;
        }
        public List<ProductPropertys> GetProductPropertys(string prodNo){
            string str_query = GetSQLSelect();
            str_query += $" WHERE ProductPropertys.ProdNo = @ProdNo";
            str_query += " ORDER BY ProductPropertys.PropertyNo";
            DynamicParameters pram = new DynamicParameters();
            pram.Add("ProdNo", prodNo);
            var model = dpr.ReadAll<ProductPropertys>(str_query, pram);
            return model;
        }
        /// <summary>
        /// 取得指定商品的屬性資料列表
        /// </summary>
        /// <param name="prodNo">商品編號</param>
        /// <returns></returns>
        public List<Propertys> GetProductPropertyList(string prodNo){
            var model = new List<Propertys>();
            string str_query = GetSQLSelect();
            str_query += $" WHERE ProductPropertys.ProdNo = @ProdNo";
            str_query += " ORDER BY ProductPropertys.PropertyNo";
            DynamicParameters pram = new DynamicParameters();
            pram.Add("ProdNo", prodNo);
            model = dpr.ReadAll<ProductPropertys>(str_query, pram)
                    .Select(x => new Propertys(){
                        PropertyNo = x.PropertyNo,
                        PropertyName = x.PropertyName    
                    }).ToList();
            return model;
        }
        /// <summary>
        /// 取得指定商品的指定屬性的資料列表
        /// </summary>
        /// <param name="prodNo"></param>
        /// <param name="propNo"></param>
        /// <returns></returns>
        public List<SelectListItem> GetProductPropertys(string prodNo , string propNo){
            var model = new List<SelectListItem>();
            string str_query = GetSQLSelect();
            str_query += $" WHERE ProductPropertys.ProdNo = @ProdNo And ProductPropertys.PropertyNo = @PropNo";
            DynamicParameters pram = new DynamicParameters();
            pram.Add("ProdNo", prodNo);
            pram.Add("PropNo", propNo);
            var data = dpr.ReadSingle<ProductPropertys>(str_query, pram);
            if(data!=null){

                model = data.PropertyValue.Split(',').ToList().Select(x=>new SelectListItem(){
                    Text = x,
                    Value = x
                }).ToList();
            }
            return model;
        }

         public string GetProductSpec(string prodNo)
        {
            string str_value = "";
            var model = GetProductPropertys(prodNo);
            foreach (var item in model)
            {
                str_value += $"{item.PropertyName}:{item.PropertyValue} ";
            }
            return str_value.Trim();
        }
    
    }
}

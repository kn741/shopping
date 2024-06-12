using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace shopping.Models
{
    public class z_sqlProductStatusNo : DapperSql<Categorys>
    {
        public z_sqlProductStatusNo()
        {
            OrderByColumn = SessionService.SortColumn;
            OrderByDirection = SessionService.SortDirection;
            DefaultOrderByColumn = "ParentNo ASC, SortNo ASC, CategoryNo ASC";
            DefaultOrderByDirection = "";
            if (string.IsNullOrEmpty(OrderByColumn)) OrderByColumn = DefaultOrderByColumn;
            if (string.IsNullOrEmpty(OrderByDirection)) OrderByDirection = DefaultOrderByDirection;
        }

        public override List<string> GetSearchColumns()
        {
            List<string> searchColumn;
            searchColumn = new List<string>() {
                    "ProductStatus.StatusNo",
                    "ProductStatus.StatusName",
                    "ProductStatus.Remark"
                     };
            return searchColumn;
        }

        public List<SelectListItem> GetDropDownList(bool textIncludeValue = false)
        {
            string str_query = "SELECT ";
            if (textIncludeValue) str_query += $"StatusNo + ' ' + ";
            str_query += "StatusName AS Text , StatusNo AS Value FROM ProductStatus ";
            str_query += GetSQLWhere();
            str_query += "ORDER BY StatusNo";
            var model = dpr.ReadAll<SelectListItem>(str_query);
            return model;
        }

        public Categorys GetData(string StatusNo)
        {
            var model = new Categorys();
            using var dpr = new DapperRepository();
            string sql_query = GetSQLSelect();
            string sql_where = "WHERE StatusNo = @StatusNo ";
            sql_query += sql_where;
            sql_query += GetSQLOrderBy();
            DynamicParameters parm = new DynamicParameters();
            if (!string.IsNullOrEmpty(sql_where))
            {
                //自定義的 Weher Parm 參數
                parm.Add("StatusNo", StatusNo);
            }
            model = dpr.ReadSingle<Categorys>(sql_query, parm);
            return model;
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace shopping.Models
{
    public class z_sqlPayments : DapperSql<Payments>
    {
        public z_sqlPayments()
        {
            OrderByColumn = SessionService.SortColumn;
            OrderByDirection = SessionService.SortDirection;
            DefaultOrderByColumn = "Payments.PaymentNo";
            DefaultOrderByDirection = "ASC";
            if (string.IsNullOrEmpty(OrderByColumn)) OrderByColumn = DefaultOrderByColumn;
            if (string.IsNullOrEmpty(OrderByDirection)) OrderByDirection = DefaultOrderByDirection;
        }

        /// <summary>
        /// 下拉式選單
        /// </summary>
        /// <param name="textIncludeValue"></param>
        /// <returns></returns>
        public List<SelectListItem> GetDropDownList(bool textIncludeValue = false)
        {
            string str_query = "SELECT ";
            if (textIncludeValue) str_query += $"PaymentNo + ' ' + ";
            str_query += "PaymentName AS Text , PaymentNo AS Value FROM Payments ";
            str_query += GetSQLWhere();
            str_query += "ORDER BY PaymentNo";
            var model = dpr.ReadAll<SelectListItem>(str_query);
            return model;
        }

    }
}
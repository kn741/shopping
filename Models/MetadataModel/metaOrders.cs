using System;
using System.Collections.Generic;

namespace shopping.Models
{
    [ModelMetadataType(typeof(z_metaOrders))]
    public partial class Orders
    {
        [NotMapped]
        [Display(Name = "訂單狀態")]
        public string? StatusName { get; set; }
        [NotMapped]
        [Display(Name = "付款方式")]
        public string? PaymentName { get; set; }
        [NotMapped]
        [Display(Name = "出貨方式")]
        public string? ShippingName { get; set; }
    }
}

public class z_metaOrders
{
    [Key]
    public int Id { get; set; }
    [Display(Name = "訂單編號")]
    public string? SheetNo { get; set; }
    [Display(Name = "訂單日期")]
    public DateTime SheetDate { get; set; }
    [Display(Name = "狀態編號")]
    public string? StatusCode { get; set; }
    [Display(Name = "是否結案")]
    public bool IsClosed { get; set; }
    [Display(Name = "是否合法")]
    public bool IsValid { get; set; }
    [Display(Name = "客戶編號")]
    public string? CustNo { get; set; }
    [Display(Name = "客戶編號")]
    public string? CustName { get; set; }
    [Display(Name = "付款方式編號")]
    public string? PaymentNo { get; set; }
    [Display(Name = "出貨方式編號")]
    public string? ShippingNo { get; set; }
    [Display(Name = "收貨人姓名")]
    public string? ReceiverName { get; set; }
    [Display(Name = "收貨人信箱")]
    public string? ReceiverEmail { get; set; }
    [Display(Name = "收貨人地址")]
    public string? ReceiverAddress { get; set; }
    [Display(Name = "未稅金額")]
    public int OrderAmount { get; set; }
    [Display(Name = "訂單稅額")]
    public int TaxAmount { get; set; }
    [Display(Name = "訂單總額")]
    public int TotalAmount { get; set; }
    [Display(Name = "備註說明")]
    public string? Remark { get; set; }
    [Display(Name = "唯一編號")]
    public string? GuidNo { get; set; }
}
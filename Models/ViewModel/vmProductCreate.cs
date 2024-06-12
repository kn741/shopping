using System.ComponentModel.DataAnnotations;

/// <summary>
/// 上架商品用 ViewModel
/// </summary>
public class vmProductCreate
{
    [Display(Name = "商品名稱")]
    [Required(ErrorMessage = "商品名稱不可空白!!")]
    public string? ProdName { get; set; } = "";
    [Display(Name = "廠牌名稱")]
    [Required(ErrorMessage = "廠牌名稱不可空白!!")]
    public string? BrandName { get; set; } = "";
    [Display(Name = "廠牌系列")]
    [Required(ErrorMessage = "廠牌系列不可空白!!")]
    public string? BrandSeriesName { get; set; } = "";
    [Display(Name = "商品狀態")]
    [Required(ErrorMessage = "商品狀態不可空白!!")]
    public string? StatusNo { get; set; } = "";
    [Display(Name = "商品分類")]
    [Required(ErrorMessage = "商品分類不可空白!!")]
    public string? CategoryNo { get; set; } = "";
    [Display(Name = "商品價格")]
    [Required(ErrorMessage = "商品價格不可空白!!")]
    public decimal SalePrice { get; set; } = 0;
    [Display(Name = "折扣價格")]
    [Required(ErrorMessage = "折扣價格不可空白!!")]
    public decimal DiscountPrice { get; set; } = 0;
    [Display(Name = "市場價格")]
    [Required(ErrorMessage = "市場價格不可空白!!")]
    public decimal MarketPrice { get; set; } = 0;
    [Display(Name = "成本價格")]
    [Required(ErrorMessage = "成本價格不可空白!!")]
    public decimal CostPrice { get; set; } = 0;
    [Display(Name = "商品描述")]
    public string? ContentText { get; set; } = "";
    [Display(Name = "商品規格")]
    public string? SpecificationText { get; set; } = "";
}
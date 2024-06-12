using System;
using System.Collections.Generic;

namespace shopping.Models
{
    [ModelMetadataType(typeof(metaProductPropertys))]
    public partial class ProductPropertys
    {
        [NotMapped]
        [Display(Name = "商品名稱")]
        public string? ProdName { get; set; }
        [NotMapped]
        [Display(Name = "屬性名稱")]
        public string? PropertyName { get; set; }
    }
}
public class metaProductPropertys
    {   
        [Key]
        public int Id { get; set; }
        [Display(Name = "選取")]
        public bool IsSelect { get; set; }
        [Display(Name = "商品編號")]
        public string? ProdNo { get; set; }
        [Display(Name = "屬性編號")]
        public string? PropertyNo { get; set; }
        [Display(Name = "屬性值")]
        public string? PropertyValue { get; set; }
        [Display(Name = "備註")]
        public string? Remark { get; set; }
    }
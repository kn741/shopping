using System.Net.Mime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
/// <summary>
/// 購物商城服務類別
/// </summary>
public class ShopService:BaseClass
{   
    /// <summary>
    /// 取得商品主要圖片位置
    /// </summary>
    /// <param name="prodNo">商品編號</param>
    /// <returns></returns>
    public static string GetProductImage(string prodNo){
        return $"~/images/products/{prodNo}/{prodNo}.jpg";
    }
    /// <summary>
    /// 取得商品指定圖片位置
    /// </summary>
    /// <param name="prodNo">商品編號</param>
    /// <param name="imgName">商品圖片名稱</param>
    /// <returns></returns>
    public static string GetProductImage(string prodNo,string imgName){
        return $"~/images/products/{prodNo}/{imgName}";
    }


    /// <summary>
    /// 取得商品所有圖片清單，不含主要商品圖片
    /// </summary>
    /// <param name="prodNo"></param>
    /// <returns></returns>
    public List<string> GetProductsImageList(string prodNo){
        string str_prod_file = $"{prodNo}.jpg";
        List<string> ImageList = new List<string>();
        var rootFolder = Directory.GetCurrentDirectory();
        string str_path = $"wwwroot\\images\\products\\{prodNo}";
        string str_dir = Path.Combine(rootFolder, str_path);
        var fileLists = Directory.GetFiles(str_dir).ToList();
        if (fileLists.Count > 0){
            foreach (var item in fileLists){
                string str_name = Path.GetFileName(item);
                if (str_name!= str_prod_file){
                    ImageList.Add(GetProductImage(prodNo,str_name));
                }
            }
        }
        return ImageList;
    }
}

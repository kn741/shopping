using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace shopping.Areas.User.Controllers
{
    public class ProductController : Controller
    {
        [Area("User")]
        [HttpGet]
        [Login(RoleList = "User,Mis")]
        public IActionResult Init()
        {
            SessionService.SetPrgInit();
            return RedirectToAction("Index", ActionService.Controller, new { area = ActionService.Area });
        }

        [Area("User")]
        [HttpGet]
        [Login(RoleList = "User,Mis")]
        public IActionResult Index()
        {
            SessionService.SetProgramInfo("", "你的商品");
            string id = SessionService.UserNo;
            using var product = new z_sqlProducts();
            var model = product.GetVendorData(id);
            return View(model);
        }

        [Area("User")]
        [HttpGet]
        [Login(RoleList = "User,Mis")]
        public IActionResult ProductCreate()
        {
            SessionService.SetProgramInfo("", "上架商品");
            string id = SessionService.UserNo;
            return View();
        }


        [Area("User")]
        [HttpPost]
        [Login(RoleList = "User,Mis")]
        public IActionResult ProductCreate(vmProductCreate model)
        {
            if(!ModelState.IsValid){return View(model);}
            SessionService.SetProgramInfo("", "上架商品");
            string id = SessionService.UserNo;
            using var product = new z_sqlProducts();
            string productNo = product.CreateNewProduct(model);
            return RedirectToAction("ProductPhotoUpload", ActionService.Controller, new { area = ActionService.Area, prodNo = productNo });
        }


        [Area("User")]
        [HttpGet]
        [Login(RoleList = "User,Mis")]
        public IActionResult ProductEdit(string id)
        {
            SessionService.SetProgramInfo("", "商品資訊編輯");
            string UserNo = SessionService.UserNo;
            using var product = new z_sqlProducts();
            var model = product.GetData(id);
            if (model == null || !model.VendorNo.Equals(UserNo))
                return RedirectToAction("Index", ActionService.Controller, new { area = ActionService.Area });
            SessionService.StringValue1 = model.ProdNo;
            return View(model);
        }


        [Area("User")]
        [HttpPost]
        [Login(RoleList = "User,Mis")]
        public IActionResult ProductEdit(Products model)
        {
            if(!ModelState.IsValid){return View(model);}
            SessionService.SetProgramInfo("", "商品資訊編輯");
            string id = SessionService.UserNo;
            using var product = new z_sqlProducts();
            product.UpdateProduct(model);
            SessionService.StringValue1="";
            return RedirectToAction("Index", ActionService.Controller, new { area = ActionService.Area });
        }


        [Area("User")]
        [HttpGet]
        [Login(RoleList = "User,Mis")]
        public IActionResult Delete(string id)
        {
            SessionService.SetProgramInfo("", "你的商品");
            using var product = new z_sqlProducts();
            product.DeleteProduct(id);
            string[] files = { "", "01", "02", "03", "04" };
            // 取得目前專案資料夾目錄路徑
            string FileNameOnServer = Directory.GetCurrentDirectory();
            FileNameOnServer += $"\\wwwroot\\images\\products\\{id}\\{id}";
            // 刪除已存在檔案
            foreach (string fileName in files)
            {
                if (System.IO.File.Exists(FileNameOnServer + fileName + ".jpg"))
                    System.IO.File.Delete(FileNameOnServer + fileName + ".jpg");
            }
            return RedirectToAction("Index", ActionService.Controller, new { area = ActionService.Area });
        }

        [Area("User")]
        [HttpGet]
        [Login(RoleList = "User,Mis")]
        public IActionResult ProductPhotoUpload(string prodNo)
        {
            SessionService.SetProgramInfo("", "商品照片上傳");
            SessionService.SetProductNo(prodNo);
            return View();
        }

        [Area("User")]
        [HttpPost]
        [Login(RoleList = "User,Mis")]
        public IActionResult ProductPhotoUpload(IFormFile file, IFormFile file1, IFormFile file2, IFormFile file3, IFormFile file4)
        {
            string prodNo = SessionService.ProductNo;
            IFormFile[] files = { file, file1, file2, file3, file4 };
            string[] fileNames = { "", "01", "02", "03", "04" };
            for (int i = 0; i < files.Length; i++)
            {
                if (!prodNo.Equals("") && files[i] != null && files[i].Length > 0)
                {
                    // 取得目前專案資料夾目錄路徑
                    string DirectionayName = Directory.GetCurrentDirectory();
                    DirectionayName += $"\\wwwroot\\images\\products\\{prodNo}";

                    // 建立資料夾
                    if (!System.IO.Directory.Exists(DirectionayName))
                        System.IO.Directory.CreateDirectory(DirectionayName);

                    string FileNameOnServer = DirectionayName + $"\\{prodNo}";
                    // 刪除已存在檔案
                    if (System.IO.File.Exists(FileNameOnServer + fileNames[i] + ".jpg"))
                        System.IO.File.Delete(FileNameOnServer + fileNames[i] + ".jpg");
                    // 建立一個串流物件

                    using var stream = System.IO.File.Create(FileNameOnServer + fileNames[i] + ".jpg");
                    // 將檔案寫入到此串流物件中
                    files[i].CopyTo(stream);
                }
            }
            return RedirectToAction("Index", ActionService.Controller, new { area = ActionService.Area });
        }


    }
}
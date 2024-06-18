namespace shopping.Controllers
{
    public class UserController : Controller
    {
        /// <summary>
        /// 登出系統
        /// </summary>
        /// <returns></returns> 
        [HttpGet]
        [AllowAnonymous()]
        public IActionResult Logout()
        {
            SessionService.IsLogin = false;
            SessionService.UserNo = "";
            SessionService.UserName = "";
            CartService.NewLotNo();
            return RedirectToAction("Login", "User", new { area = "" });
        }

        /// <summary>
        /// 登入系統
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous()]
        public IActionResult Login()
        {   
            //判斷是否已登入
            if (SessionService.IsLogin) return RedirectToAction("Index", "Home", new { area = "" });
            
            SessionService.SetProgramInfo("", "使用者");
            ActionService.SetActionName(enAction.Login);
            vmLogin model = new vmLogin();
            return View(model);
        }

        /// <summary>
        /// 登入系統
        /// </summary>
        /// <param name="model">使用者輸入的資料模型</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous()]
        public IActionResult Login(vmLogin model)
        {
            if (!ModelState.IsValid) return View(model);
            using var user = new z_sqlUsers();
            if (!user.CheckLogin(model))
            {
                ModelState.AddModelError("UserNo", "登入帳號或密碼輸入不正確!!");
                model.UserNo = "";
                model.Password = "";
                return View(model);
            }

            //判斷使用者角色，進入不同的首頁
            var data = user.GetData(model.UserNo);
            if (data.RoleNo == "Mis" || data.RoleNo == "User")
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            if (data.RoleNo == "Member"){
                CartService.MergeCart();
                return RedirectToAction("Index", "Home", new { area = "" });
            }
            //角色不正確,引發自定義錯誤,並重新輸入
            ModelState.AddModelError("UserNo", "登入帳號角色設定不正確!!");
            model.UserNo = "";
            model.Password = "";
            return View(model);
        }
        [HttpGet]
        [AllowAnonymous()]
        public IActionResult Register()
        {
            //判斷是否已登入
            if (SessionService.IsLogin) return RedirectToAction("Index", "Home", new { area = "" });
            
            SessionService.SetProgramInfo("", "使用者");
            ActionService.SetActionName(enAction.Register);
            vmRegister model = new vmRegister();
            model.GenderCode = "M";
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous()]
        public IActionResult Register(vmRegister model)
        {
            if (!ModelState.IsValid) return View(model);

            //檢查登入帳號及電子信箱是否有重覆
            using var user = new z_sqlUsers();
            if (!user.CheckRegisterUserNo(model.UserNo))
            {
                ModelState.AddModelError("UserNo", "登入帳號重覆註冊!!");
                return View(model);
            }
            if (!user.CheckRegisterEmail(model.Email))
            {
                ModelState.AddModelError("Email", "電子信箱重覆註冊!!");
                return View(model);
            }
            //新增未審核的使用者記錄
            string str_code = user.RegisterNewUser(model);

            //寄出驗證信
            using var sendEmail = new SendMailService();
            string str_message = user.CheckMailValidateCode(str_code);
            if (string.IsNullOrEmpty(str_message))
            {
                var userData = user.GetValidateUser(str_code);
                var mailObject = new MailObject();
                mailObject.MailTime = DateTime.Now;
                mailObject.ValidateCode = str_code;
                mailObject.UserNo = userData.UserNo;
                mailObject.UserName = userData.UserName;
                mailObject.ToName = userData.UserName;
                mailObject.ToEmail = userData.ContactEmail;
                mailObject.ValidateCode = str_code;
                mailObject.ReturnUrl = $"{ActionService.HttpHost}/User/RegisterValidate/{str_code}";

                str_message = sendEmail.UserRegister(mailObject);
                if (string.IsNullOrEmpty(str_message))
                {
                    str_message = "您的註冊資訊已建立，請記得收信完成驗證流程!!";
                }
            }
            //顯示註冊訊息
            SessionService.StringValue1 = "WebLogin";
            SessionService.MessageText = str_message;
            return RedirectToAction("Index", "Message", new { area = "" });
        }

        [HttpGet]
        [AllowAnonymous()]
        public IActionResult RegisterValidate(string id)
        {
            using var user = new z_sqlUsers();
            SessionService.StringValue1 = "WebLogin";
            SessionService.MessageText = user.RegisterConfirm(id); ;
            return RedirectToAction("Index", "Message", new { area = "" });
        }
        [HttpGet]
        [AllowAnonymous()]
        public IActionResult Forget()
        {
            SessionService.SetProgramInfo("", "使用者");
            ActionService.SetActionName(enAction.Forget);
            vmForget model = new vmForget();
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous()]
        public IActionResult Forget(vmForget model)
        {
            //1.檢查輸入資料是否合格
            if (!ModelState.IsValid) return View(model);
            using var user = new z_sqlUsers();

            //2.檢查帳號是否存在,存在則設定新的密碼也設定狀態為未審核
            string str_code = user.Forget(model.UserNo);
            if (string.IsNullOrEmpty(str_code))
            {
                ModelState.AddModelError("UserNo", "查無帳號或電子信箱資訊!!");
                return View(model);
            }

            //3.寄出忘記密碼驗證信
            using var sendEmail = new SendMailService();
            string str_message = user.CheckMailValidateCode(str_code);
            if (string.IsNullOrEmpty(str_message))
            {
                var userData = user.GetValidateUser(str_code);
                var mailObject = new MailObject();
                mailObject.MailTime = DateTime.Now;
                mailObject.ValidateCode = str_code;
                mailObject.UserNo = userData.UserNo;
                mailObject.UserName = userData.UserName;
                mailObject.ToName = userData.UserName;
                mailObject.ToEmail = userData.ContactEmail;
                mailObject.ValidateCode = str_code;
                mailObject.Password = userData.Password;
                mailObject.ReturnUrl = $"{ActionService.HttpHost}/User/ForgetValidate/{str_code}";

                str_message = sendEmail.UserForget(mailObject);
                if (string.IsNullOrEmpty(str_message))
                {
                    str_message = "您重設密碼的要求已受理，請記得收信完成重設密碼的流程!!!";
                }
            }

            //顯示註冊訊息
            SessionService.StringValue1 = "WebLogin";
            SessionService.MessageText = str_message;
            return RedirectToAction("Index", "Message", new { area = "" });
        }

        [HttpGet]
        [AllowAnonymous()]
        public IActionResult ForgetValidate(string id)
        {
            using var user = new z_sqlUsers();
            //更新使用者狀態為已審核
            string str_message = user.ForgetConfirm(id);
            //顯示重設密碼訊息
            SessionService.StringValue1 = "WebLogin";
            SessionService.MessageText = str_message;
            return RedirectToAction("Index", "Message", new { area = "" });
        }
    }
}
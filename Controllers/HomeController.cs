using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using shopping.Models;

namespace shopping.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    [HttpGet]
    public IActionResult Question()
    {
        using var question = new z_sqlQuestions();
        var model = question.GetDataList();
        return View(model);
    }

    [HttpGet]
    public IActionResult Contact()
    {
        var model = new vmContact();
        return View(model);
    }

    [HttpPost]
    public IActionResult Contact(vmContact model)
    {
        if (!ModelState.IsValid) return View(model);

        //寄信給系統管理者及訊息提交者
        using var sendEmail = new SendMailService();

        var mailObject = new MailObject();
        mailObject.MailTime = DateTime.Now;
        mailObject.UserNo = "";
        mailObject.UserName = "";
        mailObject.ToName = model.ContactorName;
        mailObject.ToEmail = model.ContactorEmail;
        mailObject.MailSubject = model.ContactorSubject;
        mailObject.MailContent = model.ContactorMessage;

        SessionService.MessageText = sendEmail.ContactUs(mailObject);

        //顯示完成訊息
        SessionService.StringValue1 = "ShopIndex";
        if (string.IsNullOrEmpty(SessionService.MessageText))
            SessionService.MessageText = "您的留言訊息已提交，我們會使用電子信箱方式盡速回覆您的問題。";

        return RedirectToAction("Index", "Message", new { area = "" });
    }
}

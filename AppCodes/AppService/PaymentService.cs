using System;
public static class PaymentService{
    /// <summary>
    /// HttpContextAccessor 物件
    /// </summary>
    /// <returns></returns>
    public static IHttpContextAccessor _contextAccessor { get; set; } = new HttpContextAccessor();
    /// <summary>
    /// HttpContext 物件
    /// </summary>
    public static HttpContext? _context { get { return _contextAccessor.HttpContext; } }

    /// <summary>
    /// 會員姓名
    /// </summary>
    /// <value></value>
    public static string MemberName
    {
        get
        {
            string str_value = "";
            if (_context != null) str_value = _context.Session.Get<string>("MemberName");
            if (str_value == null) str_value = "";
            return str_value;
        }
        set
        { _context?.Session.Set<string>("MemberName", value); }
    }

    /// <summary>
    /// 會員信箱
    /// </summary>
    /// <value></value>
    public static string MemberEmail
    {
        get
        {
            string str_value = "";
            if (_context != null) str_value = _context.Session.Get<string>("MemberEmail");
            if (str_value == null) str_value = "";
            return str_value;
        }
        set
        { _context?.Session.Set<string>("MemberEmail", value); }
    }

    /// <summary>
    /// 會員電話
    /// </summary>
    /// <value></value>
    public static string MemberTel
    {
        get
        {
            string str_value = "";
            if (_context != null) str_value = _context.Session.Get<string>("MemberTel");
            if (str_value == null) str_value = "";
            return str_value;
        }
        set
        { _context?.Session.Set<string>("MemberTel", value); }
    }

    /// <summary>
    /// 會員地址
    /// </summary>
    /// <value></value>
    public static string MemberAddress
    {
        get
        {
            string str_value = "";
            if (_context != null) str_value = _context.Session.Get<string>("MemberAddress");
            if (str_value == null) str_value = "";
            return str_value;
        }
        set
        { _context?.Session.Set<string>("MemberAddress", value); }
    }

    /// <summary>
    /// 收件人資料與購買人資料不同
    /// </summary>
    /// <value></value>
    public static string IsDiffenceMember
    {
        get
        {
            string str_value = "";
            if (_context != null) str_value = _context.Session.Get<string>("IsDiffenceMember");
            if (str_value == null) str_value = "";
            return str_value;
        }
        set
        { _context?.Session.Set<string>("IsDiffenceMember", value); }
    }

    /// <summary>
    /// 收件人姓名
    /// </summary>
    /// <value></value>
    public static string ReceiveName
    {
        get
        {
            string str_value = "";
            if (_context != null) str_value = _context.Session.Get<string>("ReceiveName");
            if (str_value == null) str_value = "";
            return str_value;
        }
        set
        { _context?.Session.Set<string>("ReceiveName", value); }
    }

    /// <summary>
    /// 收件人信箱
    /// </summary>
    /// <value></value>
    public static string ReceiveEmail
    {
        get
        {
            string str_value = "";
            if (_context != null) str_value = _context.Session.Get<string>("ReceiveEmail");
            if (str_value == null) str_value = "";
            return str_value;
        }
        set
        { _context?.Session.Set<string>("ReceiveEmail", value); }
    }

    /// <summary>
    /// 收件人電話
    /// </summary>
    /// <value></value>
    public static string ReceiveTel
    {
        get
        {
            string str_value = "";
            if (_context != null) str_value = _context.Session.Get<string>("ReceiveTel");
            if (str_value == null) str_value = "";
            return str_value;
        }
        set
        { _context?.Session.Set<string>("ReceiveTel", value); }
    }

    /// <summary>
    /// 收件人地址
    /// </summary>
    /// <value></value>
    public static string ReceiveAddress
    {
        get
        {
            string str_value = "";
            if (_context != null) str_value = _context.Session.Get<string>("ReceiveAddress");
            if (str_value == null) str_value = "";
            return str_value;
        }
        set
        { _context?.Session.Set<string>("ReceiveAddress", value); }
    }

    /// <summary>
    /// 付款方式
    /// </summary>
    /// <value></value>
    public static string PaymentNo
    {
        get
        {
            string str_value = "";
            if (_context != null) str_value = _context.Session.Get<string>("PaymentNo");
            if (str_value == null) str_value = "";
            return str_value;
        }
        set
        { _context?.Session.Set<string>("PaymentNo", value); }
    }

    /// <summary>
    /// 運送方式
    /// </summary>
    /// <value></value>
    public static string ShippingNo
    {
        get
        {
            string str_value = "";
            if (_context != null) str_value = _context.Session.Get<string>("ShippingNo");
            if (str_value == null) str_value = "";
            return str_value;
        }
        set
        { _context?.Session.Set<string>("ShippingNo", value); }
    }

    /// <summary>
    /// 訂單備註
    /// </summary>
    /// <value></value>
    public static string Remark
    {
        get
        {
            string str_value = "";
            if (_context != null) str_value = _context.Session.Get<string>("Remark");
            if (str_value == null) str_value = "";
            return str_value;
        }
        set
        { _context?.Session.Set<string>("Remark", value); }
    }

    public static void SetPaymentData(vmOrders model){
        MemberName = model.MemberName;
        MemberEmail = model.MemberEmail;
        MemberTel = model.MemberTel;
        MemberAddress = model.MemberAddress;
        IsDiffenceMember = model.IsDiffenceMember;
        ReceiveName = model.ReceiveName;
        ReceiveEmail = model.ReceiveEmail;
        ReceiveTel = model.ReceiveTel;
        ReceiveAddress = model.ReceiveAddress;
        PaymentNo = model.PaymentNo;
        ShippingNo = model.ShippingNo;
        Remark = model.Remark;
    }
    public static vmOrders GetPaymentData(){
        vmOrders model = new vmOrders();
        model.MemberName = MemberName;
        model.MemberEmail = MemberEmail;
        model.MemberTel = MemberTel;
        model.MemberAddress = MemberAddress;
        model.IsDiffenceMember = IsDiffenceMember;
        model.ReceiveName = ReceiveName;
        model.ReceiveEmail = ReceiveEmail;
        model.ReceiveTel = ReceiveTel;
        model.ReceiveAddress = ReceiveAddress;
        model.PaymentNo = PaymentNo;
        model.ShippingNo = ShippingNo;
        model.Remark = Remark;
        return model;
    }
}
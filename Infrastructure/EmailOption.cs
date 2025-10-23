namespace OnlineBookStore.Infrastructure
{
    public class EmailOptions
    {
        public string SmtpServer { get; set; } = string.Empty;   // 邮件服务器地址，如 smtp.gmail.com
        public int Port { get; set; }                            // 邮件服务器端口，一般是 465 或 587
        public bool UseSsl { get; set; }                         // 是否使用 SSL 加密
        public string UserName { get; set; } = string.Empty;     // 登录邮箱用户名（通常与发件人邮箱相同）
        public string Password { get; set; } = string.Empty;     // 登录邮箱密码或授权码
        public string SenderEmail { get; set; } = string.Empty;  // 发件人邮箱
        public string SenderName { get; set; } = string.Empty;   // 发件人显示名称（如 “系统通知”）
    }
}

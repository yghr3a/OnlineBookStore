using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using OnlineBookStore.Infrastructure;

public class EmailSendService
{
    private readonly EmailOptions _options;

    public EmailSendService(IOptions<EmailOptions> options)
    {
        _options = options.Value;
    }

    /// <summary>
    /// 发送邮箱方法
    /// </summary>
    /// <param name="to"></param>
    /// <param name="subject"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    public async Task SendAsync(string to, string subject, string body)
    {
        // TODO: 后面研究研究这部分的异常处理
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_options.SenderName, _options.SenderEmail));
        message.To.Add(new MailboxAddress("", to));
        message.Subject = subject;

        message.Body = new TextPart("html") { Text = body };

        using var client = new SmtpClient();
        await client.ConnectAsync(_options.SmtpServer, _options.Port, _options.UseSsl);
        await client.AuthenticateAsync(_options.UserName, _options.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
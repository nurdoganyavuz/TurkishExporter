using MailKit.Security;

namespace Core.Utilities.MailService.Abstract
{
    public interface IMailServer
    {
        bool BypassSslErrors { get; set; }
        string DisplayName { get; set; }
        string Password { get; set; }
        int Port { get; set; }
        SecureSocketOptions SecureSocket { get; set; }
        string SmtpServer { get; set; }
        string Username { get; set; }
    }
}
using Core.Utilities.MailService.Abstract;
using MailKit.Security;

namespace Core.Utilities.MailService.Models
{
    public class MailServer : IMailServer
    {
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SmtpServer { get; set; }  
        public int Port { get; set; }
        public SecureSocketOptions SecureSocket { get; set; }
        public bool BypassSslErrors { get; set; }
    }
}

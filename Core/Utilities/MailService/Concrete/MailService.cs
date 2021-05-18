using Core.Utilities.MailService.Abstract;
using Core.Utilities.MailService.Models;
using Core.Utilities.Validations;
using MailKit.Net.Smtp;
using MimeKit;
using System;

namespace Core.Utilities.MailService.Concrete
{
    public class MailService : IMailService
    {
        private readonly IMailServer _mailServer;
        public MailService(IMailServer mailServer)
        {
            _mailServer = mailServer;
        }
        public bool SendMail(MailInformation mailInformation)
        {
            try
            {
                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = mailInformation.Body
                };
                var message = new MimeMessage()
                {
                    Subject = mailInformation.Subject,
                    Body = bodyBuilder.ToMessageBody()
                };
                message.From.Add(new MailboxAddress(_mailServer.DisplayName, _mailServer.Username));
                using (var client = new SmtpClient())
                {
                    client.Connect(_mailServer.SmtpServer, _mailServer.Port, _mailServer.SecureSocket);
                    client.Authenticate(_mailServer.Username, _mailServer.Password);
                    if (_mailServer.BypassSslErrors)
                        client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    foreach (var item in mailInformation.MailPerson)
                    {
                        if (EmailValidation.IsValidEmail(item.EmailAddress))
                        {
                            message.To.Clear();
                            message.To.Add(MailboxAddress.Parse(item.EmailAddress));
                            client.Send(message);
                        }
                    }
                    client.Disconnect(true);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

using Core.Utilities.MailService.Models;

namespace Core.Utilities.MailService.Abstract
{
    public interface IMailService
    {
        bool SendMail(MailInformation mailInformation);
    }
}

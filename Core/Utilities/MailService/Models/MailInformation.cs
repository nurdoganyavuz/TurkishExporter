using System.Collections.Generic;

namespace Core.Utilities.MailService.Models
{
    public class MailInformation
    {
        public List<MailPerson> MailPerson { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}

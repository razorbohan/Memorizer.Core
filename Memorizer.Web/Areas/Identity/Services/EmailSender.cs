using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Memorizer.Web.Areas.Identity.Services
{
    public class EmailSender : IEmailSender
    {
        public SendGridClient Client { get; }

        public EmailSender(string sendGridKey)
        {
            Client = new SendGridClient(sendGridKey);
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            var msg = new SendGridMessage
            {
                From = new EmailAddress("master@memorizer.ga", "Memo Master"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));

            msg.SetClickTracking(false, false);

            return Client.SendEmailAsync(msg);
        }
    }
}

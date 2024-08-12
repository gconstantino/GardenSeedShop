using SendGrid;
using SendGrid.Helpers.Mail;

namespace BestShop.MyHelpers
{
    public class EmailSender
    {
        public static async Task  SendEmail(string toEmail, string username, string subject, string message)
        {
            string apikey = "";
            var client = new SendGridClient(apikey);

            var from = new EmailAddress("admin@gardenseedshop.com", "gardenseedshop.com");
            var to = new EmailAddress(toEmail, username);
            var plainTextContent = message;
            var htmlContent = "";

            var msg = MailHelper.CreateSingleEmail(
                from, to, subject, plainTextContent, htmlContent);

            var response = await client.SendEmailAsync(msg);

        }
    }
}

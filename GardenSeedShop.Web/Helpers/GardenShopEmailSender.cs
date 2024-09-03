using GardenSeedShop.Web.Helpers;
using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace GardenSeedShop.Web
{
	public class GardenShopEmailSender : IGardenShopEmailSender
	{
		private readonly IConfiguration _config;
		private readonly IFeatureChecker _featureChecker;

		public GardenShopEmailSender(IFeatureChecker featureChecker, IConfiguration config)
		{
			_config = config;
			_featureChecker = featureChecker;
		}

		public async Task SendEmail(string toEmail, string username, string subject, string message)
		{
			if (_featureChecker.IsEmailSenderEnabled())
			{
				string apikey = _config[AppSettingsNames.SendGridApiKey];

				if (!string.IsNullOrEmpty(apikey))
				{
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
	}
}

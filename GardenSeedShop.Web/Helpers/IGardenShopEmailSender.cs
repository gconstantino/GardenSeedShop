
namespace GardenSeedShop.Web
{
	public interface IGardenShopEmailSender
	{
		Task SendEmail(string toEmail, string username, string subject, string message);
	}
}
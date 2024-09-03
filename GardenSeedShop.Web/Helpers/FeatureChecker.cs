namespace GardenSeedShop.Web.Helpers
{
	public class FeatureChecker : IFeatureChecker
	{
        private readonly IConfiguration _configuration;

        public FeatureChecker(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool IsEmailSenderEnabled()
        {
            string value = _configuration[AppSettingsNames.Features.EnableEmailSender];

            if (bool.TryParse(value, out bool result))
            {
                return result;
            }

            return false;
        }
    }
}

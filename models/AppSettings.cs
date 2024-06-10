using Microsoft.Extensions.Configuration;

namespace B3Alert {
    public class AppSettings {
        private EmailSetting EmailSetting { get; set; }
        private ApiSetting ApiSetting { get; set; }

        public AppSettings() {
            string basePath = Directory.GetCurrentDirectory();
            var builder = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile($"{basePath}/configuration/appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration configuration = builder.Build();

            setEmailSettings(configuration);
            setApiSettings(configuration);
        }

        private void setApiSettings(IConfiguration configuration) {
            var apiSettings = configuration.GetSection("QuotationApi");
            ApiSetting = new ApiSetting(
                baseUri: apiSettings["BaseUri"],
                apiKey: apiSettings["ApiKey"]
            );
        }

        private void setEmailSettings(IConfiguration configuration) {
            var emailSettingsInfo = configuration.GetSection("EmailSettings");
            var authType = emailSettingsInfo["AuthType"];
            if (authType == "OAuth") {
                var oAuthSetting = emailSettingsInfo.GetSection("OAuth");
                EmailSetting = new OAuthMailSetting(
                    clientId: oAuthSetting["ClientId"],
                    clientSecret: oAuthSetting["ClientSecret"],
                    authUri: oAuthSetting["AuthUri"],
                    recipientEmail: emailSettingsInfo["RecipientEmail"],
                    senderEmail: emailSettingsInfo["SenderEmail"],
                    smtpServer: emailSettingsInfo["SmtpServer"],
                    smtpPort: int.Parse(emailSettingsInfo["SmtpPort"]),
                    senderUser: emailSettingsInfo["SenderUser"]
                );
            } else {
                var basicSetting = emailSettingsInfo.GetSection("Basic");
                EmailSetting = new BasicMailSetting(
                    recipientEmail: basicSetting["RecipientEmail"],
                    senderEmail: basicSetting["SenderEmail"],
                    smtpServer: basicSetting["SmtpServer"],
                    smtpPort: int.Parse(basicSetting["SmtpPort"]),
                    senderUser: emailSettingsInfo["SenderUser"],
                    senderPassword: basicSetting["SenderPassword"]
                );
            }
        }

        public bool isBasicAuth() {
            return EmailSetting is BasicMailSetting;
        }

        public EmailSetting getEmailConfig() {
            return EmailSetting;
        }

        public ApiSetting getApiConfig() {
            return ApiSetting;
        }
    }
}

using Microsoft.Extensions.Configuration;

class AppSettings {
    private EmailSetting EmailSettings { get; set; }
    private EmailOAuthSetting EmailOAuthSettings { get; set; }

    public AppSettings() {
        LoadConfigs();
    }

    private void LoadConfigs() {
        var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        IConfiguration configuration = builder.Build();
        
//        var emailSetting = new EmailSetting();
        var emailSettingsInfo = configuration.GetSection("EmailSettings");
        var authType = emailSettingsInfo["AuthType"];
        if (authType == "OAuth") {
            var oAuthSetting = emailSettingsInfo.GetSection("OAuth");
            EmailOAuthSettings = new EmailOAuthSetting(
                clientId: oAuthSetting["ClientId"],
                clientSecret: oAuthSetting["ClientSecret"]
            );
        } else {
            var basicSetting = emailSettingsInfo.GetSection("Basic");
            EmailSettings = new EmailSetting(
                recipient: basicSetting["RecipientEmail"],
                sender: basicSetting["SenderEmail"],
                smtpServer: basicSetting["SmtpServer"],
                smtpPort: int.Parse(basicSetting["SmtpPort"]),
                senderPassword: basicSetting["SenderPassword"]
            );
        }
    }

    public bool isBasicAuth() {
        return EmailSettings != null;
    }

    public EmailSetting getEmailBasicConfig() {
        return EmailSettings;
    }

    public EmailOAuthSetting GetEmailOAuthSetting() {
        return EmailOAuthSettings;
    }
}

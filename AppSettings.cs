using Microsoft.Extensions.Configuration;

class AppSettings {
    private EmailSetting EmailSetting { get; set; }

    public AppSettings() {
        var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        IConfiguration configuration = builder.Build();

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
}

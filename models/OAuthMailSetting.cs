class OAuthMailSetting : EmailSetting
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string AuthUri { get; set; }

    public OAuthMailSetting(string clientId, string clientSecret, string authUri, string recipientEmail, string senderEmail, string smtpServer, int smtpPort, string senderUser) : base(recipientEmail, senderEmail, smtpServer, smtpPort, senderUser)
    {
        ClientId = clientId;
        ClientSecret = clientSecret;
        AuthUri = authUri;
    }
}

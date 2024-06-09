class BasicMailSetting : EmailSetting
{
    public string SenderPassword { get; set; }

    public BasicMailSetting(string recipientEmail, string senderEmail, string smtpServer, int smtpPort, string senderUser, string senderPassword) : base(recipientEmail, senderEmail, smtpServer, smtpPort, senderUser)
    {
        SenderPassword = senderPassword;
    }
}

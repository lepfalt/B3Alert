class EmailSetting {
    public string RecipientEmail { get; set; }
    public string SmtpServer { get; set; }
    public int SmtpPort { get; set; }
    public string SenderEmail { get; set; }
    public string SenderPassword { get; set; }

    public EmailSetting(string recipient, string sender, string smtpServer, int smtpPort, string senderPassword) {
        RecipientEmail = recipient;
        SenderEmail = sender;
        SmtpServer = smtpServer;
        SmtpPort = smtpPort;
        SenderPassword = senderPassword;
    }
}

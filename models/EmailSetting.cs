namespace B3Alert {
    public abstract class EmailSetting {
        public string RecipientEmail { get; set; }
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SenderEmail { get; set; }
        public string SenderUser { get; set; }

        public EmailSetting(string recipientEmail, string senderEmail, string smtpServer, int smtpPort, string senderUser) {
            RecipientEmail = recipientEmail;
            SenderEmail = senderEmail;
            SmtpServer = smtpServer;
            SmtpPort = smtpPort;
            SenderUser = senderUser;
        }
    }
}

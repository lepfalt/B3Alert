using System;
using System.IO;
using System.Globalization;
using MimeKit;
using MailKit.Net.Smtp;
using Google.Apis.Auth.OAuth2;
using MailKit.Security;
using Google.Apis.Util.Store;

namespace B3Alert
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: dotnet run -- <stock> <sell-price> <buy-price>");
                return;
            }

            string stock = args[0];
            decimal sellPrice = decimal.Parse(args[1], CultureInfo.InvariantCulture); // InvariantCulture garante que o ponto será considerado como separador decimal
            decimal buyPrice = decimal.Parse(args[2], CultureInfo.InvariantCulture);

            Console.WriteLine(stock + sellPrice + buyPrice);

            var appSettings = new AppSettings();
            if (appSettings.isBasicAuth()) {
                var emailConfig = appSettings.getEmailBasicConfig();
                SendEmailAlert(emailConfig, null);
            } else {
                var emailOAuthConfig = appSettings.GetEmailOAuthSetting();
                SendEmailAlert(null, emailOAuthConfig);
            }

            Console.ReadLine();
        }

        private static void SendEmailAlert(EmailSetting? emailConfig, EmailOAuthSetting? emailOAuthConfig) {
            var message = new MimeMessage();
            message.Subject = "Teste B3Alert";
            message.Body = new TextPart("plain") {
                Text = "Isso é um teste"
            };

            if (emailConfig != null) {
                message.From.Add(new MailboxAddress("", emailConfig.SenderEmail));
                message.To.Add(new MailboxAddress("", emailConfig.RecipientEmail));
                ConnectInBasicMode(emailConfig, message);
            } else {
                ConnectInOAuthMode(emailOAuthConfig, message);
            }

            Console.WriteLine("Email enviado.");
        }

        private static void ConnectInOAuthMode(EmailOAuthSetting emailOAuthConfig, MimeMessage message) {
            var clientSecrets = new ClientSecrets {
                ClientId = emailOAuthConfig.ClientId,
                ClientSecret = emailOAuthConfig.ClientSecret
            };

            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                clientSecrets,
                new[] { "https://mail.google.com/" },
                "lepfalt.dev",
                CancellationToken.None,
                new FileDataStore("token.json", true)
            );

            var client = new SmtpClient();
            client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);

            var oauth2 = new SaslMechanismOAuth2(credential.Result.UserId, credential.Result.Token.AccessToken);
            client.Authenticate(oauth2);

            message.From.Add(new MailboxAddress("", "lepfalt.dev@gmail.com"));
            message.To.Add(new MailboxAddress("", "lepfalt.dev@gmail.com"));
            client.Send(message);
            Console.WriteLine("E-mail enviado com sucesso!");
        }

        private static void ConnectInBasicMode(EmailSetting emailConfig, MimeMessage message) {
            var client = new SmtpClient();
            client.Connect(emailConfig.SmtpServer, emailConfig.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
            client.Authenticate(emailConfig.SenderEmail, emailConfig.SenderPassword);
            client.Send(message);
            client.Disconnect(true);
        }
    }
}

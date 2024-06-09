using System.Globalization;
using MimeKit;
using MailKit.Net.Smtp;
using Google.Apis.Auth.OAuth2;
using MailKit.Security;
using Google.Apis.Util.Store;

namespace B3Alert
{
    public class MailAlert {

        public static void Send(AppSettings appSettings, string messageToBeSend) {
            var message = new MimeMessage();
                message.Subject = "B3 Alert";
                message.Body = new TextPart("plain") {
                    Text = messageToBeSend
                };

                var mailSetting = appSettings.getEmailConfig();
                message.From.Add(new MailboxAddress("", mailSetting.SenderEmail));
                message.To.Add(new MailboxAddress("", mailSetting.RecipientEmail));

                if (appSettings.isBasicAuth()) {
                    ConnectInBasicMode(mailSetting as BasicMailSetting, message);
                } else {
                    ConnectInOAuthMode(mailSetting as OAuthMailSetting, message);
                }

                Console.WriteLine("E-mail enviado com sucesso!");
        }

        private static void ConnectInOAuthMode(OAuthMailSetting emailOAuthConfig, MimeMessage message) {
            var clientSecrets = new ClientSecrets {
                ClientId = emailOAuthConfig.ClientId,
                ClientSecret = emailOAuthConfig.ClientSecret
            };

            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                clientSecrets,
                new[] { emailOAuthConfig.AuthUri },
                emailOAuthConfig.SenderUser,
                CancellationToken.None,
                new FileDataStore("token.json", true)
            );

            var client = new SmtpClient();
            client.Connect(emailOAuthConfig.SmtpServer, emailOAuthConfig.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);

            var oauth2 = new SaslMechanismOAuth2(credential.Result.UserId, credential.Result.Token.AccessToken);
            client.Authenticate(oauth2);

            client.Send(message);
        }

        private static void ConnectInBasicMode(BasicMailSetting emailConfig, MimeMessage message) {
            var client = new SmtpClient();
            client.Connect(emailConfig.SmtpServer, emailConfig.SmtpPort, SecureSocketOptions.StartTls);
            client.Authenticate(emailConfig.SenderEmail, emailConfig.SenderPassword);
            client.Send(message);
            client.Disconnect(true);
        }
    }
}

using MimeKit;
using MailKit.Net.Smtp;
using Google.Apis.Auth.OAuth2;
using MailKit.Security;
using Google.Apis.Util.Store;

namespace B3Alert
{
    public class MailAlert {
        private static string BasePath = Directory.GetCurrentDirectory();
        private const string TokenPath = "configuration/tokens";

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
        }

        private static void ConnectInOAuthMode(OAuthMailSetting emailOAuthConfig, MimeMessage message) {
            try {
                AuthAndSendMessage(emailOAuthConfig, message);
            } catch (AuthenticationException exception) {
                Console.WriteLine($"Falha para obter o token. Exception: {exception}");

                Console.WriteLine("Iniciando retentativa");
                string basePath = Directory.GetCurrentDirectory();
                string directoryPath = $"{BasePath}/{TokenPath}";
                string[] files = Directory.GetFiles(directoryPath);
                foreach(string path in files) {
                    File.Delete(path);
                }

                AuthAndSendMessage(emailOAuthConfig, message);
            } catch (Exception exception) {
                Console.WriteLine($"Falha ao enviar email com OAuth. Exception: {exception}");
            }
        }

        private static void AuthAndSendMessage(OAuthMailSetting emailOAuthConfig, MimeMessage message) {
            var clientSecrets = new ClientSecrets {
                ClientId = emailOAuthConfig.ClientId,
                ClientSecret = emailOAuthConfig.ClientSecret
            };

            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                clientSecrets,
                new[] { emailOAuthConfig.AuthUri },
                emailOAuthConfig.SenderUser,
                CancellationToken.None,
                new FileDataStore($"/{TokenPath}", true)
            );

            var client = new SmtpClient();
            client.Connect(emailOAuthConfig.SmtpServer, emailOAuthConfig.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);

            var oauth2 = new SaslMechanismOAuth2(credential.Result.UserId, credential.Result.Token.AccessToken);
            client.Authenticate(oauth2);

            client.Send(message);

            Console.WriteLine("E-mail enviado com sucesso!");
        }

        private static void ConnectInBasicMode(BasicMailSetting emailConfig, MimeMessage message) {
            try {
                var client = new SmtpClient();

                client.Connect(emailConfig.SmtpServer, emailConfig.SmtpPort, SecureSocketOptions.StartTls);
                client.Authenticate(emailConfig.SenderEmail, emailConfig.SenderPassword);
                client.Send(message);
                client.Disconnect(true);

                Console.WriteLine("E-mail enviado com sucesso!");
            } catch (Exception exception) {
                Console.WriteLine($"Falha ao enviar email com basic auth. Exception: {exception}");
            }
        }
    }
}

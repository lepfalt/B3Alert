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
            SendEmailAlert(appSettings, getBuyMessage(stock, buyPrice, (decimal)100.0));
            SendEmailAlert(appSettings, getSellMessage(stock, sellPrice, (decimal)200.0));

            Console.ReadLine();
        }

        private static string getBuyMessage(string stock, decimal buyPrice, decimal actualPrice) {
            return $"A ação {stock} está abaixo ou igual ao preço desejado de compra. O valor desejado é de R${buyPrice} e o valor atual é de R${actualPrice}.";
        }

        private static string getSellMessage(string stock, decimal sellPrice, decimal actualPrice) {
            return $"A ação {stock} está acima ou igual ao preço desejado de venda. O valor desejado é de R${sellPrice} e o valor atual é de R${actualPrice}.";
        }

        private static void SendEmailAlert(AppSettings appSettings, string messageToBeSend) {
            var message = new MimeMessage();
            message.Subject = "B3 Alert";
            message.Body = new TextPart("plain") {
                Text = messageToBeSend
            };

            var emailConfig = appSettings.getEmailConfig();
            message.From.Add(new MailboxAddress("", emailConfig.SenderEmail));
            message.To.Add(new MailboxAddress("", emailConfig.RecipientEmail));

            if (appSettings.isBasicAuth()) {
                ConnectInBasicMode(emailConfig as BasicMailSetting, message);
            } else {
                ConnectInOAuthMode(emailConfig as OAuthMailSetting, message);
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

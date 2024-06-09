using System.Globalization;

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
            // var mailConfig = appSettings.getEmailConfig();
            MailAlert.Send(appSettings, getBuyMessage(stock, buyPrice, (decimal)100.0));
            MailAlert.Send(appSettings, getSellMessage(stock, sellPrice, (decimal)200.0));

            Console.ReadLine();
        }

        private static string getBuyMessage(string stock, decimal buyPrice, decimal actualPrice) {
            return $"A ação {stock} está abaixo ou igual ao preço desejado de compra. O valor desejado é de R${buyPrice} e o valor atual é de R${actualPrice}.";
        }

        private static string getSellMessage(string stock, decimal sellPrice, decimal actualPrice) {
            return $"A ação {stock} está acima ou igual ao preço desejado de venda. O valor desejado é de R${sellPrice} e o valor atual é de R${actualPrice}.";
        }
    }
}

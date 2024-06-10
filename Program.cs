using System.Globalization;

namespace B3Alert
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Use o seguinte formato: dotnet run -- <stock> <sell-price> <buy-price>");
                return;
            }

            string stock = args[0];
            decimal sellPrice = ParseToDecimal(args[1]);
            decimal buyPrice = ParseToDecimal(args[2]);

            Console.WriteLine($"Monitorando ativo {stock} - Preço base venda: {sellPrice}, Preço base compra: {buyPrice}..");

            await MonitoringQuotation(
                new AppSettings(),
                stock,
                sellPrice,
                buyPrice
            );
        }

        private static decimal ParseToDecimal(string value) {
            try {
                return decimal.Parse(value, CultureInfo.InvariantCulture); // InvariantCulture garante que o ponto será considerado como separador decimal
            } catch (FormatException) {
                throw new InputRequireException($"Argumento inválido: {value}. Para correção use o seguinte formato: dotnet run -- <stock:string> <sell-price:decimal> <buy-price:decimal>");
            }
        }

        private static async Task MonitoringQuotation(AppSettings appSettings, string stock, decimal sellPrice, decimal buyPrice) {
            while(true) {
                decimal currentPrice = await QuotationApi.GetStockPrice(appSettings, stock);
                Console.WriteLine($"Cotação atual de {stock}: {currentPrice}");

                if (currentPrice > sellPrice)
                {
                    MailAlert.Send(appSettings, getSellMessage(stock, sellPrice, currentPrice));
                }
                else if (currentPrice < buyPrice)
                {
                    MailAlert.Send(appSettings, getBuyMessage(stock, buyPrice, currentPrice));
                }

                await Task.Delay(60000); // Aguardar 1 minuto antes de verificar novamente
            }
        }

        private static string getBuyMessage(string stock, decimal buyPrice, decimal actualPrice) {
            return $"A ação {stock} está abaixo do preço desejado de compra. O valor desejado é de R${buyPrice} e o valor atual é de R${actualPrice}.";
        }

        private static string getSellMessage(string stock, decimal sellPrice, decimal actualPrice) {
            return $"A ação {stock} está acima do preço desejado de venda. O valor desejado é de R${sellPrice} e o valor atual é de R${actualPrice}.";
        }
    }
}

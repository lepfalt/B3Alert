using System.Globalization;
using Newtonsoft.Json.Linq;

namespace B3Alert {
    public static class QuotationApi {
        public static async Task<decimal> GetStockPrice(AppSettings appSettings, string stockName) {
            try {
                var apiSettings = appSettings.getApiConfig();
                var client = new HttpClient();
                string apiUrl = $"{apiSettings.BaseUri}/price?symbol={stockName}&apikey={apiSettings.ApiKey}";

                var response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                var data = JObject.Parse(responseBody);
                var price = data["price"];
                decimal latestPrice = decimal.Parse(price.ToString(), CultureInfo.InvariantCulture);   

                return Math.Round(latestPrice, 2);
            } catch (Exception exception) {
                throw new QuotationApiException("Erro ao bater na API de cotações.", exception);
            }
        }
    }
}

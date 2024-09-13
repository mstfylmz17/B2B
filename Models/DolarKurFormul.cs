using DataAccessLayer.Concrate;
using Newtonsoft.Json.Linq;

namespace VNNB2B.Models
{
    public class DolarKurFormul
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public decimal GetDolarKuru(int i)
        {
            var apiUrl = "https://api.exchangerate-api.com/v4/latest/USD";

            var response = _httpClient.GetStringAsync(apiUrl).Result;

            var data = JObject.Parse(response);

            var tryKuru = data["rates"]["TRY"].Value<decimal>();

            return tryKuru;
        }
        public decimal GetEuroKuru(int i)
        {
            var apiUrl = $"https://api.exchangerate-api.com/v4/latest/EUR";

            var response = _httpClient.GetStringAsync(apiUrl).Result;

            var data = JObject.Parse(response);

            var tryKuru = data["rates"]["TRY"].Value<decimal>();

            return tryKuru;
        }
    }
}

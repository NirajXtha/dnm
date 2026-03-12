using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Service.Scheme.Khalti
{


    public class KhaltiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://khalti.com/api";
        private readonly string _authKey;

        public KhaltiService(string authKey)
        {
            _authKey = authKey;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Key", _authKey);
        }

        /// <summary>
        /// Loads funds into a Khalti user’s account.
        /// </summary>
        public async Task<string> LoadFundsAsync(string user, int amount, string remarks, string reference)
        {
            var payload = new Dictionary<string, string>
        {
            { "user", user },
            { "amount", amount.ToString() },  // amount in paisa
            { "remarks", remarks },
            { "reference", reference }
        };

            var response = await _httpClient.PostAsync(
                $"{_baseUrl}/v2/fund/merchantload/",
                new FormUrlEncodedContent(payload)
            );

            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Checks the status of a fund load.
        /// </summary>
        public async Task<string> CheckTransactionStatusAsync(string reference, int amount)
        {
            var payload = new Dictionary<string, string>
        {
            { "reference", reference },
            { "amount", amount.ToString() }
        };

            var response = await _httpClient.PostAsync(
                $"{_baseUrl}/v2/fund/loadstatus/",
                new FormUrlEncodedContent(payload)
            );

            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Retrieves a paginated list of transactions for the merchant.
        /// </summary>
        public async Task<string> ListTransactionsAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/merchant-transaction/");
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Retrieves details of a single transaction using its idx.
        /// </summary>
        public async Task<string> GetTransactionAsync(string idx)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/merchant-transaction/{idx}/");
            return await response.Content.ReadAsStringAsync();
        }
    }

}


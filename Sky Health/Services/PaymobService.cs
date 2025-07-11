using Sky_Health.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sky_Health.Services
{
    public class PaymobService : IPaymobService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly ILogger<PaymobService> _logger;

        public PaymobService(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<PaymobService> logger)
        {
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
        }

        public async Task<string> CreatePaymentUrl(Order order)
        {
            try
            {
                var paymobSettings = _configuration.GetSection("Paymob");
                string apiKey = paymobSettings["ApiKey"];
                int integrationId = int.Parse(paymobSettings["IntegrationId"]);
                string iframeId = paymobSettings["IframeId"];

                var authRequest = new { api_key = apiKey };
                var authResponse = await PostAsync<PaymobAuthResponse>("https://accept.paymob.com/api/auth/tokens", authRequest);
                if (authResponse == null || string.IsNullOrEmpty(authResponse.Token))
                {
                    _logger.LogError("Paymob authentication failed.");
                    return null;
                }

                var orderRequest = new
                {
                    auth_token = authResponse.Token,
                    delivery_needed = "false",
                    amount_cents = (int)(order.TotalAmount * 100),
                    currency = "EGP",
                    merchant_order_id = order.Id.ToString(),
                    items = order.OrderItems.Select(item => new {
                        name = item.ProductName,
                        amount_cents = (int)(item.UnitPrice * 100),
                        description = "N/A",
                        quantity = item.Quantity
                    }).ToList()
                };
                var orderResponse = await PostAsync<PaymobOrderResponse>("https://accept.paymob.com/api/ecommerce/orders", orderRequest);
                if (orderResponse == null)
                {
                    _logger.LogError("Paymob order registration failed for Order ID: {OrderId}", order.Id);
                    return null;
                }

                var paymentKeyRequest = new
                {
                    auth_token = authResponse.Token,
                    amount_cents = (int)(order.TotalAmount * 100),
                    expiration = 3600,
                    order_id = orderResponse.Id,
                    billing_data = new
                    {
                        apartment = "NA",
                        email = "not-available@example.com",
                        floor = "NA",
                        first_name = order.CustomerName?.Split(' ').FirstOrDefault() ?? "Unknown",
                        street = order.Address ?? "NA",
                        building = "NA",
                        phone_number = order.PhoneNumber ?? "NA",
                        shipping_method = "NA",
                        postal_code = "NA",
                        city = "NA",
                        country = "EG",
                        last_name = order.CustomerName?.Split(' ').LastOrDefault() ?? "User",
                        state = "NA"
                    },
                    currency = "EGP",
                    integration_id = integrationId
                };
                var paymentKeyResponse = await PostAsync<PaymobPaymentKeyResponse>("https://accept.paymob.com/api/acceptance/payment_keys", paymentKeyRequest);

                if (paymentKeyResponse == null || string.IsNullOrWhiteSpace(paymentKeyResponse.Token))
                {
                    _logger.LogError("Failed to generate payment key from Paymob.");
                    return null;
                }

                return $"https://accept.paymob.com/api/acceptance/iframes/{iframeId}?payment_token={paymentKeyResponse.Token}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred in CreatePaymentUrl service.");
                return null;
            }
        }
        private async Task<T> PostAsync<T>(string url, object payload)
        {
            var jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);

            var responseString = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Paymob API call to {Url} failed with status code {StatusCode}. Response: {Response}", url, response.StatusCode, responseString);
                return default;
            }

            return JsonConvert.DeserializeObject<T>(responseString);
        }

        private class PaymobAuthResponse { public string Token { get; set; } }
        private class PaymobOrderResponse { public int Id { get; set; } }
        private class PaymobPaymentKeyResponse { public string Token { get; set; } }
    }
}
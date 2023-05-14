using System;
using System.Net.Http;
using System.Threading.Tasks;
using AspnetRunBasics.Extensions;
using AspnetRunBasics.Models;

namespace AspnetRunBasics.Services;

public class BasketService : IBasketService
{
    private readonly HttpClient _client;

    public BasketService(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<BasketModel> GetBasket(string userName)
    {
        var response = await _client.GetAsync($"/Basket/{userName}");
        return await response.Content.ReadAsAsync<BasketModel>();
    }

    public async Task<BasketModel> UpdateBasket(BasketModel model)
    {
        var response = await _client.PostAsJsonAsync($"/Basket", model);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadAsAsync<BasketModel>();
        else
        {
            throw new Exception("Something went wrong when calling api.");
        }
    }

    public async Task CheckoutBasket(BasketCheckoutModel model)
    {
        var response = await _client.PostAsJsonAsync($"/Basket/Checkout", model);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Something went wrong when calling api.");
        }
    }
}
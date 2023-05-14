using Basket.API.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Basket.API.Repositories;
public class BasketRepository:IBasketRepository
{
    private readonly IDistributedCache _redisCache;
    public BasketRepository(IDistributedCache redisCache)
    {
        _redisCache = redisCache ?? throw new ArgumentNullException(nameof(redisCache));
    }

    public async Task<ShoppingCart?> GetBasket(string? userName)
    {
        var basket = await _redisCache.GetStringAsync(userName);
        return String.IsNullOrEmpty(basket) ? null : JsonConvert.DeserializeObject<ShoppingCart>(basket);
    }

    public async Task<ShoppingCart?> UpdateBasket(ShoppingCart basket)
    {
        if (basket.UserName == null) return null;
        await _redisCache.SetStringAsync(basket.UserName, JsonConvert.SerializeObject(basket));
        return await GetBasket(basket.UserName);

    }

    public async Task DeleteBasket(string? userName)
    {
        await _redisCache.RemoveAsync(userName);
    }
}

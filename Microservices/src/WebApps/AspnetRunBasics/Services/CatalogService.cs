using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AspnetRunBasics.Extensions;
using AspnetRunBasics.Models;
using Microsoft.Extensions.Logging;

namespace AspnetRunBasics.Services;

public class CatalogService : ICatalogService
{
    private readonly HttpClient _client;        

    public CatalogService(HttpClient client, ILogger<CatalogService> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<IEnumerable<CatalogModel>> GetCatalog()
    {
        var response = await _client.GetAsync("/Catalog");
        return await response.Content.ReadAsAsync<List<CatalogModel>>();
    }

    public async Task<CatalogModel> GetCatalog(string id)
    {
        var response = await _client.GetAsync($"/Catalog/{id}");
        return await response.Content.ReadAsAsync<CatalogModel>();
    }

    public async Task<IEnumerable<CatalogModel>> GetCatalogByCategory(string category)
    {
        var response = await _client.GetAsync($"/Catalog/GetProductByCategory/{category}");
        return await response.Content.ReadAsAsync<List<CatalogModel>>();
    }

    public async Task<CatalogModel> CreateCatalog(CatalogModel model)
    {            
        var response = await _client.PostAsJsonAsync($"/Catalog", model);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadAsAsync<CatalogModel>();
        else
        {
            throw new Exception("Something went wrong when calling api.");
        }
    }        
}
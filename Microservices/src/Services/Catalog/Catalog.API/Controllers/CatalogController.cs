using Catalog.API.Entitys;
using Catalog.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers
{
    [Route("api/v1/Catalog")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(IProductRepository productRepository, ILogger<CatalogController> logger)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productRepository.GetProducts();
            return Ok(products);
        }

        
        [HttpGet("{id:length(24)}", Name = "GetProduct")]
        public async Task<IActionResult> GetProductById(string id)
        {
            var product = await _productRepository.GetProduct(id);
            if (product == null)
            {
                _logger.LogError($"Product with id: {id}, not found.");
                return NotFound();
            }
            return Ok(product);
        }

        
        [HttpGet("[action]/{categoryName}", Name = "GetProductByCategory")]
        public async Task<IActionResult> GetProductByCategory(string categoryName)
        {
            var products = await _productRepository.GetProductByCategory(categoryName);
            return Ok(products);
        }
        [HttpGet("[action]/{name}", Name = "GetProductByName")]
        public async Task<IActionResult> GetProductByName(string name)
        {
            var products = await _productRepository.GetProductByName(name);
            return Ok(products);
        }
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            await _productRepository.CreateProduct(product);
            return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] Product product)
        {
            return Ok(await _productRepository.UpdateProduct(product));
        }
        [HttpDelete("{id:length(24)}", Name = "DeleteProduct")]
        public async Task<IActionResult> DeleteProductById(string id)
        {
            return Ok(await _productRepository.DeleteProduct(id));
        }
        
    }
}

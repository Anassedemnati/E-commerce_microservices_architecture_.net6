using Discount.API.Entities;
using Discount.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Discount.API.Controller;

[Route("api/v1/[controller]")]
[ApiController]
public class DiscountController : ControllerBase
{
    private readonly IDiscountRepository _repository;

    public DiscountController(IDiscountRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }
    [HttpGet("{productName}", Name = "GetDiscount")]
    [ProducesResponseType(typeof(Coupon), StatusCodes.Status200OK)]
    public async Task<ActionResult<Coupon>> GetDiscount(string productName)
    {
        var coupon = await _repository.GetDiscount(productName);
        return Ok(coupon);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Coupon), StatusCodes.Status200OK)]
    public async Task<ActionResult<Coupon>> CreateDiscount([FromBody] Coupon coupon)
    {
        await _repository.CreateDiscount(coupon);
        return CreatedAtRoute("GetDiscount", new { productName = coupon.ProductName }, coupon);
    }

    [HttpPut]
    [ProducesResponseType(typeof(Coupon), StatusCodes.Status200OK)]
    public async Task<ActionResult<Coupon>> UpdateDiscount([FromBody] Coupon coupon)
    {
        return Ok(await _repository.UpdateDiscount(coupon));
    }

    [HttpDelete("{productName}", Name = "DeleteDiscount")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteDiscount(string productName)
    {
        await _repository.DeleteDiscount(productName);
        return Ok();
    }
}

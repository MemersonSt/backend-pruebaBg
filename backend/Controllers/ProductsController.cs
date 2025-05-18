using backend.business.products;
using backend.dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProducts _products;

        public ProductsController(IProducts products)
        {
            _products = products;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var products = await _products.GetAll();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("bycode/{code}")]
        public async Task<IActionResult> GetByCode(int code)
        {
            try
            {
                var products = await _products.GetByCode(code);
                if (products == null) return NotFound("No se Encontro registro");
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("bycodeproduct/{code}")]
        public async Task<IActionResult> GetByCodeProduct(string code)
        {
            try
            {
                var products = await _products.GetByCodeProduct(code);
                if (products == null) return NotFound("No se Encontro registro");
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] ProductsDTO product)
        {
            try
            {
                var newProduct = await _products.Insert(product);
                return CreatedAtAction(nameof(GetAll), new { id = newProduct.Code }, newProduct);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ProductsDTO product)
        {
            try
            {
                var updatedProduct = await _products.Update(product);
                return Ok(updatedProduct);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("/{code}")]
        public async Task<IActionResult> Delete(int code)
        {
            try
            {
                var updatedProduct = await _products.Delete(code);
                return Ok(updatedProduct);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

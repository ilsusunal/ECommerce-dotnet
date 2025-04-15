using ECommerce.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private static List<Product> _products = new()
        {
            new Product { Id = 1, Name = "Laptop", Price = 15000, Description = "Gaming Laptop" },
            new Product { Id = 2, Name = "Mouse", Price = 350, Description = "Wireless Mouse" }
        };

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_products);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound(new { message = "Product not found" });

            return Ok(product);
        }

        [HttpGet("list")]
        public IActionResult List([FromQuery] string? name, [FromQuery] string? sortBy)
        {
            var result = _products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                result = result.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));

            if (sortBy == "price")
                result = result.OrderBy(p => p.Price);
            else if (sortBy == "name")
                result = result.OrderBy(p => p.Name);

            return Ok(result);
        }


        [HttpPost]
        public IActionResult Create([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            product.Id = _products.Max(p => p.Id) + 1;
            product.CreatedAt = DateTime.UtcNow;

            _products.Add(product);

            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Product updatedProduct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingProduct = _products.FirstOrDefault(p => p.Id == id);
            if (existingProduct == null)
                return NotFound(new { message = "Product not found" });

            existingProduct.Name = updatedProduct.Name;
            existingProduct.Price = updatedProduct.Price;
            existingProduct.Description = updatedProduct.Description;

            return Ok(existingProduct); 
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(int id, [FromBody] JsonPatchDocument<Product> patchDoc)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound(new { message = "Product not found" });

            patchDoc.ApplyTo(product);

            if (!TryValidateModel(product))
            {
                return BadRequest(ModelState);
            }

            return Ok(product);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound(new { message = "Product not found" });

            _products.Remove(product);
            return NoContent(); 
        }

    }
}

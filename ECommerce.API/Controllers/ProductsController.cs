using ECommerce.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        // Mock veri – geçici ürün listesi
        private static List<Product> _products = new()
        {
            new Product { Id = 1, Name = "Laptop", Price = 15000, Description = "Gaming Laptop" },
            new Product { Id = 2, Name = "Mouse", Price = 350, Description = "Wireless Mouse" }
        };

        // GET: api/products
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_products);
        }

        // GET: api/products/1
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound(new { message = "Product not found" });

            return Ok(product);
        }
    }
}

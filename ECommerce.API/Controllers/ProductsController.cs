using ECommerce.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using ECommerce.API.Data;
using ECommerce.API.Attributes;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Connection to database
        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // To login
        [HttpGet("secure")]
        [CustomAuthorize]
        public IActionResult SecureEndpoint()
        {
            return Ok(new { message = "You have access to this protected endpoint." });
        }

        // GET: /api/products
        // Returns all products from the database
        [HttpGet]
        public IActionResult GetAll()
        {
            var products = _context.Products.ToList();
            return Ok(products);
        }

        // GET: /api/products/{id}
        // Returns a single product by ID
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
                return NotFound(new { message = "Product not found" });

            return Ok(product);
        }

        // GET: /api/products/list?name=abc&sortBy=price
        // Returns filtered and sorted product list based on query parameters
        [HttpGet("list")]
        public IActionResult List([FromQuery] string? name, [FromQuery] string? sortBy)
        {
            var result = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
                result = result.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));

            if (sortBy == "price")
                result = result.OrderBy(p => p.Price);
            else if (sortBy == "name")
                result = result.OrderBy(p => p.Name);

            return Ok(result.ToList());
        }

        // POST: /api/products
        // Creates a new product and saves it to the database
        [HttpPost]
        public IActionResult Create([FromBody] Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            product.CreatedAt = DateTime.UtcNow;

            _context.Products.Add(product);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        // PUT: /api/products/{id}
        // Replaces an existing product (full update)
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Product updatedProduct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingProduct = _context.Products.Find(id);
            if (existingProduct == null)
                return NotFound(new { message = "Product not found" });

            existingProduct.Name = updatedProduct.Name;
            existingProduct.Price = updatedProduct.Price;
            existingProduct.Description = updatedProduct.Description;

            _context.SaveChanges();

            return Ok(existingProduct);
        }

        // PATCH: /api/products/{id}
        // Partially updates an existing product using JSON Patch
        [HttpPatch("{id}")]
        public IActionResult Patch(int id, [FromBody] JsonPatchDocument<Product> patchDoc)
        {
            var product = _context.Products.Find(id);
            if (product == null)
                return NotFound(new { message = "Product not found" });

            patchDoc.ApplyTo(product);

            if (!TryValidateModel(product))
                return BadRequest(ModelState);

            _context.SaveChanges();

            return Ok(product);
        }

        // DELETE: /api/products/{id}
        // Deletes a product by ID
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
                return NotFound(new { message = "Product not found" });

            _context.Products.Remove(product);
            _context.SaveChanges();

            return NoContent();
        }
    }
}

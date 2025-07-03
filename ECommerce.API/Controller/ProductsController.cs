using HackerPlatform.Core.DTOs.E_Commerce;
using HackerPlatform.Core.Entities.E_Commerce;
using HackerPlatform.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductsController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ✅ Get all products
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var products = await _context.Products.ToListAsync();

            var result = products.Select(p => new
            {
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                ImageUrl = $"{baseUrl}/images/products/{p.ImageFileName}"
            });

            return Ok(result);
        }


        // ✅ Get product by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _context.Products
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var imageUrl = $"{baseUrl}/images/products/{product.ImageFileName}";

            var result = new ProductWithCommentsDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = imageUrl,
                Comments = product.Comments.Select(c => new CommentDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    UserId = c.UserId
                }).ToList()
            };

            return Ok(result);
        }

        // ⚠️ No authorization check - any user can add product (Privilege Escalation)
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromForm] CreateProductDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { status = false, message = "❌ Invalid data" });

            if (!dto.IsSafe())
                return BadRequest(new { status = false, message = "❌ You added invalid characters!" });

            if (dto.Price <= 0)
                return BadRequest(new { status = false, message = "❌ Price must be greater than 0" });

            string fileName = "default.png";

            if (dto.Image != null)
            {
                // ✅ التحقق من الامتداد
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var ext = Path.GetExtension(dto.Image.FileName).ToLower();

                if (!allowedExtensions.Contains(ext))
                {
                    return BadRequest(new { status = false, message = "❌ Only image files are allowed (.jpg, .jpeg, .png, .gif)" });
                }

                // ✅ التحقق من نوع الملف
                if (!dto.Image.ContentType.StartsWith("image/"))
                {
                    return BadRequest(new { status = false, message = "❌ Invalid image content type" });
                }

                fileName = $"{Guid.NewGuid()}{ext}";
                var path = Path.Combine(_env.WebRootPath, "images/products", fileName);

                using var stream = new FileStream(path, FileMode.Create);
                await dto.Image.CopyToAsync(stream);
            }

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                ImageFileName = fileName
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return Ok(new { status = true, message = "✅ Product created successfully" });
        }




        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts(string query)
        {
            query = query?.Trim() ?? string.Empty;
            query = HtmlEncode(query);

            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            var products = await _context.Products
                .Where(p => p.Name.Contains(query) || p.Description.Contains(query))
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Price,
                    ImageUrl = $"{baseUrl}/images/products/{p.ImageFileName}"
                })
                .ToListAsync();

            return Ok(new
            {
                query,
                results = products
            });
        }


        public static string HtmlEncode(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return input
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&#39;");
        }

    }
}

using System.Text.Json;
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
    public class WishlistController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WishlistController(AppDbContext context)
        {
            _context = context;
        }

        // 🧠 استخراج المستخدم الحالي من الكوكي والسيشن
        private async Task<ECommerceUser?> GetCurrentUser()
        {
            if (!Request.Cookies.TryGetValue("EUser", out var sessionId))
                return null;

            var session = await _context.UserSessions
                .FirstOrDefaultAsync(s => s.SessionToken == sessionId);

            if (session == null)
                return null;

            return await _context.ECommerceUsers.FindAsync(session.UserId);
        }

        #region Old Save Code
        //[HttpPost("add")]
        //public async Task<IActionResult> AddToWishlist(AddToWishlistDto dto)
        //{
        //    var user = await GetCurrentUser();
        //    if (user == null)
        //        return Unauthorized("Not logged in");

        //    var product = await _context.Products.FindAsync(dto.ProductId);
        //    if (product == null)
        //        return NotFound(new { status = false, message = "❌ Product not found" });

        //    var exists = await _context.WishlistItems
        //        .AnyAsync(w => w.UserId == user.Id && w.ProductId == dto.ProductId);

        //    if (exists)
        //        return BadRequest("Product already in wishlist");

        //    await _context.WishlistItems.AddAsync(new WishlistItem
        //    {
        //        UserId = user.Id,
        //        ProductId = dto.ProductId
        //    });

        //    await _context.SaveChangesAsync();
        //    return Ok("Added to wishlist");
        //} 
        #endregion

        //CSRF Attack no ckeck from content type 
        [HttpPost("add")]
        public async Task<IActionResult> AddToWishlist()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();

                if (string.IsNullOrWhiteSpace(body))
                    return BadRequest(new { status = false, message = "❌ Empty request body" });

                var dto = JsonSerializer.Deserialize<AddToWishlistDto>(body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (dto == null || dto.ProductId <= 0)
                    return BadRequest(new { status = false, message = "❌ Invalid product ID" });

                var user = await GetCurrentUser();
                if (user == null)
                    return Unauthorized("Not logged in");

                var product = await _context.Products.FindAsync(dto.ProductId);
                if (product == null)
                    return NotFound(new { status = false, message = "❌ Product not found" });

                var exists = await _context.WishlistItems
                    .AnyAsync(w => w.UserId == user.Id && w.ProductId == dto.ProductId);

                if (exists)
                    return BadRequest("Product already in wishlist");

                await _context.WishlistItems.AddAsync(new WishlistItem
                {
                    UserId = user.Id,
                    ProductId = dto.ProductId
                });

                await _context.SaveChangesAsync();
                return Ok(new { status = true, message = "✅ Added to wishlist" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = false, message = "❌ Error processing request", details = ex.Message });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetWishlist()
        {
            var user = await GetCurrentUser();
            if (user == null)
                return Unauthorized("Not logged in");

            var wishlist = await _context.WishlistItems
                .Where(w => w.UserId == user.Id)
                .Include(w => w.Product)
                .ToListAsync();

            return Ok(wishlist.Select(w => new
            {
                w.Product.Id,
                w.Product.Name,
                w.Product.Price,
                w.Product.ImageFileName
            }));
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearWishlist()
        {
            var user = await GetCurrentUser();
            if (user == null)
                return Unauthorized("Not logged in");

            var items = _context.WishlistItems.Where(w => w.UserId == user.Id);
            _context.WishlistItems.RemoveRange(items);
            await _context.SaveChangesAsync();

            return Ok("Wishlist cleared");
        }

        [HttpDelete("Wishlist/{productId}")]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            var user = await GetCurrentUser();
            if (user == null)
                return Unauthorized();

            var item = await _context.WishlistItems
                .FirstOrDefaultAsync(ci => ci.ProductId == productId && ci.UserId == user.Id);

            if (item == null)
                return NotFound();

            _context.WishlistItems.Remove(item);
            await _context.SaveChangesAsync();

            return Ok(new { message = "✅ Item removed from Wishlist" });
        }
    }
}

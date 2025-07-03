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
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        // 🧠 استخراج المستخدم الحالي من الكوكي
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

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDto dto)
        {
            var user = await GetCurrentUser();
            if (user == null)
                return Unauthorized(new { status = false, message = "❌ Not logged in" });

            if (dto.Quantity < 1)
                return BadRequest(new { status = false, message = "❌ Minimum quantity is 1" });

            if (dto.Quantity > 30)
                return BadRequest(new { status = false, message = "❌ Maximam quantity is 30" });

            var product = await _context.Products.FindAsync(dto.ProductId);
            if (product == null)
                return NotFound(new { status = false, message = "❌ Product not found" });

            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.ProductId == dto.ProductId && ci.UserId == user.Id);

            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
            }
            else
            {
                var item = new CartItem
                {
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    UserId = user.Id
                };
                await _context.CartItems.AddAsync(item);
            }

            await _context.SaveChangesAsync();

            return Ok(new { status = true, message = "✅ Product added to cart" });
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var user = await GetCurrentUser();
            if (user == null)
                return Unauthorized("Not logged in");

            var cart = await _context.CartItems
                .Where(c => c.UserId == user.Id)
                .Include(c => c.Product)
                .ToListAsync();

            return Ok(cart.Select(c => new
            {
                c.Product.Id,
                c.Product.Name,
                c.Product.Price,
                c.Product.ImageFileName,
                c.Quantity,
                Total = c.Product.Price * c.Quantity
            }));
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var user = await GetCurrentUser();
            if (user == null)
                return Unauthorized("Not logged in");

            var items = _context.CartItems.Where(c => c.UserId == user.Id);
            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();

            return Ok("Cart cleared");
        }

        [HttpDelete("cart/{productId}")]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var user = await GetCurrentUser();
            if (user == null)
                return Unauthorized();

            var item = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.ProductId == productId && ci.UserId == user.Id);

            if (item == null)
                return NotFound();

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();

            return Ok(new { message = "✅ Item removed from cart" });
        }
    }
}

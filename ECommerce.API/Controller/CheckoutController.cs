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
    public class CheckoutController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CheckoutController(AppDbContext context)
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
        public async Task<IActionResult> Checkout([FromBody] ApplyCouponDto dto)
        {
            var user = await GetCurrentUser();
            if (user == null)
                return Unauthorized("Not logged in");

            var cartItems = await _context.CartItems
                .Where(c => c.UserId == user.Id)
                .Include(c => c.Product)
                .ToListAsync();

            if (!cartItems.Any())
                return BadRequest("Your cart is empty");

            var total = cartItems.Sum(item => item.Product.Price * item.Quantity);
            decimal discount = 0;

            // ✅ لو فيه كوبون، نحاول نطبقه
            if (!string.IsNullOrWhiteSpace(dto.CouponCode))
            {
                var coupon = await _context.Coupons.FirstOrDefaultAsync(c =>
                    c.Code == dto.CouponCode && c.ExpiryDate > DateTime.UtcNow);

                if (coupon == null)
                    return BadRequest("Invalid or expired coupon");

                discount = coupon.IsPercentage
                    ? total * (coupon.DiscountValue / 100)
                    : coupon.DiscountValue;

                if (discount > total)
                    discount = total;

                total -= discount;
            }

            if (user.Balance < total)
                return BadRequest("Not enough balance");

            user.Balance -= total;
            _context.CartItems.RemoveRange(cartItems);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Checkout completed successfully",
                discountApplied = discount,
                totalPaid = total,
                remainingBalance = user.Balance
            });
        }
    }
}

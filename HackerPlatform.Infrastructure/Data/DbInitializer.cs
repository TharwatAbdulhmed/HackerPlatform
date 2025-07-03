using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HackerPlatform.Core.Entities;
using HackerPlatform.Core.Entities.E_Commerce;
using HackerPlatform.Core.Entities.Questions;
using Microsoft.EntityFrameworkCore;

namespace HackerPlatform.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (!context.Coupons.Any())
            {
                var coupons = new List<Coupon>
            {
                new Coupon
                {
                    Code = "SAVE10",
                    DiscountValue = 10,
                    IsPercentage = true,
                    ExpiryDate = DateTime.UtcNow.AddMonths(1)
                },
                new Coupon
                {
                    Code = "FLAT50",
                    DiscountValue = 50,
                    IsPercentage = false,
                    ExpiryDate = DateTime.UtcNow.AddMonths(1)
                },
                new Coupon
                {
                    Code = "ZPJ555", // 😉
                    DiscountValue = 15,
                    IsPercentage = true,
                    ExpiryDate = DateTime.UtcNow.AddMonths(2)
                },
                new Coupon
                {
                    Code = "Tharwat", // 😉
                    DiscountValue = 1500,
                    IsPercentage = false,
                    ExpiryDate = DateTime.UtcNow.AddDays(1)
                }
            };

                await context.Coupons.AddRangeAsync(coupons);
                await context.SaveChangesAsync();
            }
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerPlatform.Core.Entities.E_Commerce
{
    public class Coupon
    {
        public int Id { get; set; }

        public string Code { get; set; } = string.Empty;

        public decimal DiscountValue { get; set; } // مثال: 50 = خصم 50 ريال

        public DateTime ExpiryDate { get; set; }

        public bool IsPercentage { get; set; } = false; // true = نسبة مئوية
    }

}

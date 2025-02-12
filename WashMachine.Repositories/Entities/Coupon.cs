using System;

namespace WashMachine.Repositories.Entities
{
    public partial class Coupon
    {
        public Coupon()
        {
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public decimal Discount { get; set; }
        public string ShopCode { get; set; }
        public DateTime UsedDate { get; set; }
        public bool IsUsed { get; set; }
        public string ShopName { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace WashMachine.Business.Models
{
    public class CouponModel
    {
        public CouponModel()
        {
        }

        public int Id { get; set; }
        [Required]
        public string Code { get; set; }
        public decimal Discount { get; set; }
        [Required]
        public string ShopCode { get; set; }
        [Required]
        public DateTime UsedDate { get; set; }
        public bool IsUsed { get; set; }
        public string ShopName { get; set; }
    }
}

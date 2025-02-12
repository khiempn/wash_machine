using System;

namespace WashMachine.Forms.Views.PaidBy.Service.Octopus
{
    public class OctopusResponseModel
    {
        public int Id { get; set; }
        public DateTime? InsertTime { get; set; }
        public string ShopCode { get; set; }
        public int Amount { get; set; }
    }
}

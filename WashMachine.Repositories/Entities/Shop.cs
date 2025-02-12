using System;
using System.Collections.Generic;

namespace WashMachine.Repositories.Entities
{
    public partial class Shop
    {
        public Shop()
        {
            ShopOwner = new HashSet<ShopOwner>();
            ShopComs = new HashSet<ShopCom>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string ShortName { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int? Type { get; set; }
        public int? Status { get; set; }
        public string Notes { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string SecureCode { get; set; }
        public bool? Active { get; set; }
        public int? Owner { get; set; }
        public int? InsertUser { get; set; }
        public DateTime? InsertTime { get; set; }
        public int? UpdateUser { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string Coordinate { get; set; }
        public string BackgroundPath { get; set; }
        public string LogoPath { get; set; }
        public string Location { get; set; }
        public string MachineCode { get; set; }

        public ICollection<ShopOwner> ShopOwner { get; set; }

        public ICollection<ShopCom> ShopComs { get; set; }
    }
}

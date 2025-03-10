using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WashMachine.Business.Models
{
    public class ShopModel
    {
        public int Id { get; set; }
        [Required]
        public string Code { get; set; }
        public string ShortName { get; set; }

        [Required]
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int? Type { get; set; }
        [Required]
        public int? Status { get; set; }
        public string Notes { get; set; }
        public int? ShopOwnerId { get; set; }
        public string ShopOwner { get; set; }
        public int? InsertUser { get; set; }
        public DateTime? InsertTime { get; set; }
        public int? UpdateUser { get; set; }
        public DateTime? UpdateTime { get; set; }
        public List<UserModel> UserSources { get; set; }
        public string Location { get; set; }
        public string MachineCode { get; set; }
        public string BackLink { get; set; }

        public ShopComModel DollarCom { get; set; } = new ShopComModel();
    }
}

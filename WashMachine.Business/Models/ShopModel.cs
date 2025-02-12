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
        public IFormFile BackgroundFile { get; set; }
        public string BackgroundPath { get; set; }
        public IFormFile LogoFile { get; set; }
        public string LogoPath { get; set; }

        public List<UserModel> ShopUsers { get; set; }
        public string UserIds { get; set; }
        public List<UserModel> AvailableUsers { get; set; }
        public string BackLink { get; set; }
        public bool ShopActive { get
            {
                return Status == 1;
            }
        }
        public string Location { get; set; }
        public string MachineCode { get; set; }

        public ShopComModel DollarCom { get; set; } = new ShopComModel();
        public ShopComModel CouponCom { get; set; } = new ShopComModel();
        public ShopComModel OctopusCom { get; set; } = new ShopComModel();
    }
}

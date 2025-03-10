using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WashMachine.Business.Models
{
    public class UserModel
    {
        public int Id { get; set; }

        public int Type { get; set; }

        [Required]
        public string FullName { get; set; }

        public string Address { get; set; }

        [StringLength(11)]
        public string Phone { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Notes { get; set; }

        [Required]
        public int? Status { get; set; }

        [Required]
        public int? UserType { get; set; }

        public string Password { get; set; }

        public DateTime? Birthday { get; set; }

        public bool? Active { get; set; }

        public int? InsertUser { get; set; }

        public DateTime? InsertTime { get; set; }

        public int? UpdateUser { get; set; }

        public DateTime? UpdateTime { get; set; }

        public IEnumerable<string> Rights { get; set; }

        public string BirthdayText { get; set; }

        public string BackLink { get; set; }

        public bool IsAdmin
        {
            get
            {
                return UserType == (int)UserTypes.Admin;
            }
        }

        public int TestingMode { get; set; }

        public string NewPassword { get; set; }

        public string RepeatNewPassword { get; set; }

        public ShopModel ShopOwner { get; set; }
    }
}

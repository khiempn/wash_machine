using Libraries;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WashMachine.Business.Models
{
    public class UserModel
    {
        public int Id { get; set; }

        public int Type { get; set; }

        public int TeamId { get; set; }

        public string TeamName { get; set; }

        public string ShopCode { get; set; }

        [Required]
        public string FullName { get; set; }

        public string Address { get; set; }

        [StringLength(11)]
        public string Phone { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        //[Required]
        public string Username { get; set; }

        public string Notes { get; set; }
        [Required]
        public int? Status { get; set; }
        [Required]
        public int? UserType { get; set; }
        public int? RoleType { get; set; }
        public int? WorkType { get; set; }
        public string SecureCode { get; set; }
        public string Password { get; set; }
        public string SaltKey { get; set; }
        public string Image { get; set; }
        public IFormFile ImageFile { get; set; }

        public DateTime? Birthday { get; set; }
        public bool? Active { get; set; }
        public DateTime? ResetTime { get; set; }
        public int ProcessTime { get; set; }

        public int? InsertUser { get; set; }
        public DateTime? InsertTime { get; set; }
        public int? UpdateUser { get; set; }
        public DateTime? UpdateTime { get; set; }

        public IEnumerable<string> Rights { get; set; }

        public string ErrorMessage { get; set; }

        public string BirthdayText { get; set; }

        public List<SelectItem> ListShops { get; set; }

        public List<SelectItem> ListWorkTypes { get; set; }

        public string BackLink { get; set; }

        public string UserTypeName { get {
                return EnumUtilities.GetName<UserTypes>(UserType);    
            }
        }

        public bool IsAdmin
        {
            get
            {
                return UserType == (int)UserTypes.Admin;
            }
        }

        public int TestingMode { get; set; }
    }
}

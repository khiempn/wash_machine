using System;
using System.Collections.Generic;

namespace WashMachine.Repositories.Entities
{
    public partial class User
    {
        public User()
        {
            ShopOwner = new HashSet<ShopOwner>();
        }

        public int Id { get; set; }
        public string ShopCode { get; set; }
        public string FullName { get; set; }
        public DateTime? Birthday { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Notes { get; set; }
        public int? Status { get; set; }
        public int? UserType { get; set; }
        public int? RoleType { get; set; }
        public int? WorkType { get; set; }
        public string SecureCode { get; set; }
        public string Password { get; set; }
        public string SaltKey { get; set; }
        public bool? Active { get; set; }
        public DateTime? ResetTime { get; set; }
        public string Image { get; set; }
        public int? InsertUser { get; set; }
        public DateTime? InsertTime { get; set; }
        public int? UpdateUser { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int? ProcessTime { get; set; }
        public int TestingMode { get; set; }

        public ICollection<ShopOwner> ShopOwner { get; set; }
    }
}

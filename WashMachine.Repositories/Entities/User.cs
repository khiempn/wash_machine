using System;

namespace WashMachine.Repositories.Entities
{
    public partial class User
    {
        public User()
        {
        }

        public int Id { get; set; }
        public string FullName { get; set; }
        public DateTime? Birthday { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Notes { get; set; }
        public int? Status { get; set; }
        public int? UserType { get; set; }
        public string Password { get; set; }
        public int? InsertUser { get; set; }
        public DateTime? InsertTime { get; set; }
        public int? UpdateUser { get; set; }
        public DateTime? UpdateTime { get; set; }
        public int TestingMode { get; set; }
    }
}

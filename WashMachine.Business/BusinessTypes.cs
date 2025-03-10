using System;

namespace WashMachine.Business
{
    public class Roles
    {
        public const string Admin = "Admin";
        public const string ShopOwner = "Shop Owner";
    }

    public enum UserTypes
    {
        Admin = 1,
        ShopOwner = 2
    }
}

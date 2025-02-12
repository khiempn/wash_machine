using WashMachine.Business.Models;
using System.Collections.Generic;

namespace WashMachine.Web.Models
{
    public class ManagerDataModel
    {

    }

    public class UserManagerModel : ManagerDataModel
    {
        public UserFilterModel Filter { get; set; } = new UserFilterModel();
        public List<UserModel> Users { get; set; } = new List<UserModel>();
        public int Total { get; set; }
    }

    public class ShopManagerModel : ManagerDataModel
    {
        public ShopFilterModel Filter { get; set; } = new ShopFilterModel();
        public List<ShopModel> Shops { get; set; } = new List<ShopModel>();
        public int Total { get; set; }
    }
}

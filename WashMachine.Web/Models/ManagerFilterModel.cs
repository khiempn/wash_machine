namespace WashMachine.Web.Models
{
    public class ManagerFilterModel
    {
        public string SearchCriteria { get; set; } = string.Empty;
        public string FromDate { get; set; } = string.Empty;
        public string ToDate { get; set; } = string.Empty;
    }

    public class UserFilterModel : ManagerFilterModel
    {
        public string ShopCode { get; set; } = string.Empty;
    }

    public class ShopFilterModel : ManagerFilterModel
    {
        public string ShopCode { get; set; } = string.Empty;
    }
}

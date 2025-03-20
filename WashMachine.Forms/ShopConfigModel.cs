using WashMachine.Forms.Modules.Shop.Model;

namespace WashMachine.Forms
{
    public class ShopConfigModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string ShortName { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public ShopSettingModel ShopSetting { get; set; } = new ShopSettingModel();
    }
}
using WashMachine.Octopus.OctopusSetting;
using System;
using System.Linq;

namespace WashMachine.Octopus
{
    public class OctopusRepository
    {
        private static string _appType => "COIN_";

        public static int GetPaymentOctopus(string shopCode)
        {
            shopCode = _appType + shopCode;
            using (var context = new BoxcutContext())
            {
                var entity = context.PaymentOctopus.FirstOrDefault(c => c.ShopCode == shopCode && c.PaymentStatus == null);
                if (entity == null)
                {
                    var currentTime = DateTime.Now;
                    entity = new PaymentOctopus
                    {
                        ShopCode = shopCode,
                        InsertTime = currentTime
                    };
                    context.PaymentOctopus.Add(entity);
                    context.SaveChanges();
                }
                return entity.Id;
            }
        }
        public static void SavePaid(int id, string received)
        {
            using (var context = new BoxcutContext())
            {
                var octopus = context.PaymentOctopus.FirstOrDefault(c => c.Id == id);
                if (octopus != null)
                {
                    octopus.PaymentStatus = 2; //  Pending = 1, Paid = 2, Cancel = 3
                    octopus.Received = received;
                    octopus.UpdateTime = DateTime.Now;
                    octopus.Enabled = true;
                    context.SaveChanges();
                }
            }
        }
        public static string GetOtopusInfo(int id)
        {
            using (var context = new BoxcutContext())
            {
                var octopus = context.PaymentOctopus.FirstOrDefault(c => c.Id == id);
                if (octopus != null)
                {
                    return octopus.Received;
                }
                return string.Empty;
            }
        }
        public static OctopusEmail GetSetting()
        {
            using (var context = new BoxcutContext())
            {
                var list = context.Setting.ToList();
                var email = new OctopusEmail();
                foreach(var item in list)
                {
                    if (item.Key == nameof(email.ServerEmail)) email.ServerEmail = item.Value;
                    if (item.Key == nameof(email.ServerEmailPassword)) email.ServerEmailPassword = item.Value;
                    if (item.Key == nameof(email.ServerEmailReceiver)) email.ServerEmailReceiver = item.Value;
                }
                return email;
            }
        }
    }
    public class OctopusEmail
    {
        public string ServerEmail { get; set; }
        public string ServerEmailPassword { get; set; }
        public string ServerEmailReceiver { get; set; }
    }
}

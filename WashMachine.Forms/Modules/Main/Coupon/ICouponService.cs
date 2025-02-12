using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WashMachine.Forms.Modules.Main.Coupon
{
    public interface ICouponService
    {
        Task<Dictionary<string, string>> ValidateAsync(string code);
        Task StartScanning();
        void StopScan();
        Task<bool> DropCoinAsync(string couponCode);
        Task StartScanning(Form mainForm);
        void StopScan(Form mainForm);
        Task SetCouponIsUsed(string couponCode);
    }
}

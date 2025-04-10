using WashMachine.Forms.Database.Tables.Machine;
using WashMachine.Forms.Modules.PaidBy.Service.Model;

namespace WashMachine.Forms.Modules.LaundryWashOption.Api
{
    public interface IWashApiService
    {
        void TrackingMachineError(OrderModel order, string machineName, string commandError);

        void UpdateMachineInfo(MachineModel machine);
    }
}

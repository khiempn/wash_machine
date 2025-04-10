﻿using System;
using System.Threading.Tasks;
using WashMachine.Forms.Common.Http;
using WashMachine.Forms.Database.Tables.Machine;
using WashMachine.Forms.Modules.PaidBy.Service.Model;

namespace WashMachine.Forms.Modules.LaundryWashOption.Api
{
    public class WashApiService : IWashApiService
    {
        HttpService httpService;
        ShopConfigModel shopConfig;

        public WashApiService()
        {
            httpService = new HttpService();
            shopConfig = Program.AppConfig.GetShopConfig();
        }

        public void UpdateMachineInfo(MachineModel machine)
        {
            Task.Run(() =>
            {
                httpService.Post($"{Program.AppConfig.AppHost}/Machine/SetMachineInfo", new
                {
                    ShopCode = shopConfig.Code,
                    Time = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"),
                }).GetAwaiter();
            });
        }

        public void TrackingMachineError(OrderModel order, string machineName, string commandError)
        {
            Task.Run(() =>
            {
                httpService.Post($"{Program.AppConfig.AppHost}/Machine/TrackingMachineError", new
                {
                    ShopCode = shopConfig.Code,
                    Time = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"),
                    Command = commandError,
                    PaymentId = order.PaymentId,
                    MachineName = machineName
                }).GetAwaiter();
            });
        }
    }
}

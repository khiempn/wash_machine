using AutoMapper;
using WashMachine.Business.Models;
using WashMachine.Repositories.Entities;

namespace WashMachine.Web.AppCode
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Shop, ShopModel>();
            CreateMap<Order, OrderModel>();
            CreateMap<Payment, PaymentModel>();

            CreateMap<User, UserModel>();
            CreateMap<Coupon, CouponModel>();
            CreateMap<ShopCom, ShopComModel>();
            CreateMap<Log, LogModel>();
            CreateMap<MachineCommand, MachineCommandModel>();
            CreateMap<RunCommandError, RunCommandErrorModel>();
        }
    }
}

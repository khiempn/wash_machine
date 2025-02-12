using AutoMapper;
using WashMachine.Business.Models;
using WashMachine.Repositories.Entities;

namespace WashMachine.Web.AppCode
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //CreateMap<User, UserModel>().ForMember(dest => dest.Birthday, opt => opt.MapFrom(src => TextUtilities.GetString(src.BoD)));
            CreateMap<Shop, ShopModel>();
            CreateMap<Order, OrderModel>();
            CreateMap<Payment, PaymentModel>();
            CreateMap<PaymentOctopus, PaymentModel>();

            CreateMap<User, UserModel>();
            CreateMap<Coupon, CouponModel>();
            CreateMap<ShopCom, ShopComModel>();
            CreateMap<Log, LogModel>();
            //PaymentModel
            //CreateMap<Keygen, KeygenModel>().ForMember(dest => dest.StartDateText, opt => opt.MapFrom(src => TextUtilities.GetString(src.StartDate)))
            //    .ForMember(dest => dest.EndDateText, opt => opt.MapFrom(src => TextUtilities.GetString(src.EndDate)));
        }
    }
}

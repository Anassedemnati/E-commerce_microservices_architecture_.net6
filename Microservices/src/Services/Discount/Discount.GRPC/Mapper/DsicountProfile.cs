using AutoMapper;
using Discount.GRPC.Entities;
using Discount.GRPC.Protos;

namespace Discount.GRPC.Mapper;

public class DsicountProfile : Profile
{
	public DsicountProfile()
	{
        CreateMap<Coupon, CouponModel>().ReverseMap();
    }
}

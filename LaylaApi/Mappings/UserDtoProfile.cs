using AutoMapper;
using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;

namespace LaylaApi.Mappings
{
    public class UserDtoProfile : Profile
    {
        public UserDtoProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => src.Email!.Value))

                .ForMember(dest => dest.PhoneNumber,
                    opt => opt.MapFrom(src => src.PhoneNumber!.Value))

                .ForMember(dest => dest.Language,
                    opt => opt.MapFrom(src => src.Lang != null
                    ? src.Lang.Code
                    : "en"))

                .ForMember(dest => dest.ApartmentsCount,
                    opt => opt.MapFrom(src =>
                    src.Apartments != null
                    ? src.Apartments.Count
                    : 0))

                .ForMember(dest => dest.BookingsCount,
                    opt => opt.MapFrom(src =>
                    src.Bookings != null
                    ? src.Bookings.Count
                    : 0))

                .ForMember(dest => dest.CreatedAt,
                    opt => opt.MapFrom(src => src.CreatedAt));
        }
    }
}

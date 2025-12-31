using AutoMapper;
using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;

namespace LaylaApi.Mappings
{
    public class MediaFileProfile : Profile
    {
        public MediaFileProfile() 
        {
            CreateMap<MediaFile, MediaFileDto>();

            CreateMap<MediaFileCreateDto, MediaFile>()
                .ForMember(dest => dest.UploadedAt, opt => opt.Ignore());
        }
    }
}

using AutoMapper;
using LaylaApi.Models.DtosModels.MainDtos;
using LaylaApi.Models.MainModels;

namespace LaylaApi.Mappings
{
    public class ContractProfile : Profile
    {
        public ContractProfile() 
        {
            CreateMap<Contract, ContractDto>();

            CreateMap<CreateContractDto, Contract>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsSignedByOwner, opt => opt.Ignore())
                .ForMember(dest => dest.IsSignedByRenter, opt => opt.Ignore());
        }
    }
}
